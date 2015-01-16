using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class MediaSecurityEvaluator : SecurityEvaluator, ISecurityEvaluatorFor<IMediaFile>, ISecurityEvaluatorFor<IMediaFileFolder>
    {
        private static readonly ConcurrentDictionary<string, EvaluatedPermissions> Cache = new ConcurrentDictionary<string, EvaluatedPermissions>();

        private static IDictionary<string, IMediaFilePermissions> _filePermissionsCache;
        private static IDictionary<string, IMediaFolderPermissions> _folderPermissionsCache;

        static MediaSecurityEvaluator()
        {
            DataEvents<IMediaFolderPermissions>.OnAfterAdd += Flush;
            DataEvents<IMediaFolderPermissions>.OnAfterUpdate += Flush;
            DataEvents<IMediaFolderPermissions>.OnDeleted += Flush;

            DataEvents<IMediaFilePermissions>.OnAfterAdd += Flush;
            DataEvents<IMediaFilePermissions>.OnAfterUpdate += Flush;
            DataEvents<IMediaFilePermissions>.OnDeleted += Flush;

            BuildPermissionsCache();
        }

        private static void Flush(object sender, DataEventArgs e)
        {
            Cache.Clear();

            BuildPermissionsCache();
        }

        private static void BuildPermissionsCache()
        {
            using (var data = new DataConnection())
            {
                _filePermissionsCache = data.Get<IMediaFilePermissions>().ToDictionary(p => p.KeyPath);
                _folderPermissionsCache = data.Get<IMediaFolderPermissions>().ToDictionary(p => p.KeyPath);
            }
        }

        public bool HasAccess(IMediaFile file)
        {
            var e = GetEvaluatedPermissions(file);

            return PermissionsFacade.HasAccess(e);
        }

        public bool HasAccess(IMediaFileFolder folder)
        {
            var e = GetEvaluatedPermissions(folder);

            return PermissionsFacade.HasAccess(e);
        }

        public EvaluatedPermissions GetEvaluatedPermissions(IMediaFileFolder folder)
        {
            return Cache.GetOrAdd(folder.KeyPath, g =>
            {
                IMediaFolderPermissions permission;
                _folderPermissionsCache.TryGetValue(folder.KeyPath, out permission);

                return EvaluatePermissions(permission, p => EvaluateInheritedPermissions(GetParent(folder), p));
            });
        }

        public EvaluatedPermissions GetEvaluatedPermissions(IMediaFile file)
        {
            return Cache.GetOrAdd(file.KeyPath, g =>
            {
                IMediaFilePermissions permission;
                _filePermissionsCache.TryGetValue(file.KeyPath, out permission);

                return EvaluatePermissions(permission, p => EvaluateInheritedPermissions(GetParent(file), p));
            });
        }

        private static void EvaluateInheritedPermissions(IMediaFileFolder folder, EvaluatedPermissions evaluatedPermissions)
        {
            var allowedRoles = new List<string>();
            var deniedRolews = new List<string>();

            while (folder != null)
            {
                IMediaFolderPermissions permissions;
                if (_folderPermissionsCache.TryGetValue(folder.KeyPath, out permissions))
                {
                    if (permissions.DisableInheritance)
                    {
                        break;
                    }

                    var ar = PermissionsFacade.Split(permissions.AllowedRoles);
                    var dr = PermissionsFacade.Split(permissions.DeniedRoles);

                    allowedRoles.AddRange(ar);
                    deniedRolews.AddRange(dr);
                }

                folder = GetParent(folder);
            }

            evaluatedPermissions.InheritedAllowedRules = allowedRoles.ToArray();
            evaluatedPermissions.InheritedDenieddRules = deniedRolews.ToArray();
        }

        private static IMediaFileFolder GetParent(IMediaFileFolder folder)
        {
            return GetParent(folder.StoreId, folder.Path);
        }

        private static IMediaFileFolder GetParent(IMediaFile file)
        {
            return GetParent(file.StoreId, Path.Combine(file.FolderPath, file.FileName));
        }

        private static IMediaFileFolder GetParent(string storeId, string path)
        {
            var parentPath = GetParentFolder(path);
            if (path.Equals("/", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            using (var data = new DataConnection())
            {
                var parentFolder = data.Get<IMediaFileFolder>().SingleOrDefault(f => f.StoreId == storeId && f.Path.Equals(parentPath, StringComparison.OrdinalIgnoreCase));

                return parentFolder;
            }
        }

        private static string GetParentFolder(string path)
        {
            var split = path.Split(new[] { '\\', '/' });
            if (split.Length == 1)
            {
                return "/";
            }

            var folder = String.Join("/", split.Take(split.Length - 1));

            return folder;
        }
    }
}
