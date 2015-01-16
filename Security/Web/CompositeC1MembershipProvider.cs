using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Security;

using Composite;
using Composite.Data;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Web
{
    public class CompositeC1MembershipProvider : MembershipProvider
    {
        private static readonly Func<string, Expression<Func<IMembershipUser, bool>>> UsernamePredicate = userName => (u => String.Compare(u.UserName, userName, StringComparison.OrdinalIgnoreCase) == 0);
        private static readonly Func<string, Expression<Func<IMembershipUser, bool>>> EmailPredicate = email => (u => String.Compare(u.Email, email, StringComparison.OrdinalIgnoreCase) == 0);

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { return int.MaxValue; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotSupportedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotSupportedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotSupportedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            ApplicationName = config["applicationName"];
            if (String.IsNullOrEmpty(ApplicationName))
            {
                throw new ProviderException("Application name needs to be set");
            }

            base.Initialize(name, config);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(username));
                if (user == null)
                {
                    return false;
                }

                var oldPasswordHash = PasswordHash.HashPassword(oldPassword);
                if (!PasswordHash.ValidatePassword(oldPassword, oldPasswordHash))
                {
                    return false;
                }

                var newPasswordHash = PasswordHash.HashPassword(newPassword);

                user.Password = newPasswordHash;
                user.LastPasswordChangedDate = DateTime.UtcNow;

                data.Update(user);

                return true;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            Verify.ArgumentNotNullOrEmpty(username, "username");
            Verify.ArgumentNotNullOrEmpty(email, "email");

            if (MailsFacade.ValidateMailAddress(email))
            {
                status = MembershipCreateStatus.InvalidEmail;

                return null;
            }

            using (var data = new DataConnection())
            {
                var existingUser = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(username));
                if (existingUser != null)
                {
                    status = MembershipCreateStatus.DuplicateUserName;

                    return null;
                }

                existingUser = data.Get<IMembershipUser>().Where(u => u.IsApproved == isApproved).SingleOrDefault(EmailPredicate(email));
                if (existingUser != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;

                    return null;
                }

                var id = providerUserKey == null ? Guid.NewGuid() : (Guid)providerUserKey;

                existingUser = data.Get<IMembershipUser>().SingleOrDefault(u => u.Id == id);
                if (existingUser != null)
                {
                    status = MembershipCreateStatus.DuplicateProviderUserKey;

                    return null;
                }

                var user = data.CreateNew<IMembershipUser>();

                user.Id = id;
                user.CreationDate = DateTime.UtcNow;
                user.Password = PasswordHash.HashPassword(password);
                user.ProviderName = ApplicationName;
                user.UserName = username;
                user.IsApproved = isApproved;
                user.Email = email;

                data.Add(user);

                var membershipUser = MapUser(user);

                status = MembershipCreateStatus.Success;

                return membershipUser;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(username));
                if (user == null)
                {
                    return false;
                }

                data.Delete(user);

                return true;
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetAllUsers(pageIndex, pageSize, out totalRecords, EmailPredicate(emailToMatch));
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetAllUsers(pageIndex, pageSize, out totalRecords, UsernamePredicate(usernameToMatch));
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return GetAllUsers(pageIndex, pageSize, out totalRecords, null);
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(username));
                if (user != null)
                {
                    return MapUser(user);
                }
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Guid))
            {
                throw new ArgumentException("UserKey has to be of type Guid", "providerUserKey");
            }

            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(u => u.Id == (Guid)providerUserKey);
                if (user != null)
                {
                    return MapUser(user);
                }
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(EmailPredicate(email));
                if (user != null)
                {
                    return user.UserName;
                }
            }

            return String.Empty;
        }

        public override string ResetPassword(string username, string answer)
        {
            var newPassword = GeneratePassword(6);

            return SetPassword(username, newPassword) ? newPassword : String.Empty;
        }

        public override bool UnlockUser(string userName)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(userName));
                if (user == null)
                {
                    return false;
                }

                user.IsLockedOut = false;
                user.LastActivityDate = DateTime.UtcNow;

                data.Update(user);

                return true;
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            Verify.ArgumentNotNullOrEmpty(user.UserName, "user");
            Verify.ArgumentNotNullOrEmpty(user.Email, "user");

            using (var data = new DataConnection())
            {
                var c1User = data.Get<IMembershipUser>().SingleOrDefault(u => u.Id == (Guid)user.ProviderUserKey);
                if (c1User == null)
                {
                    throw new ArgumentException(String.Format("Provider user key '{0}' doesn't exist", user.ProviderUserKey), "user");
                }

                if (!user.Email.Equals(c1User.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = data.Get<IMembershipUser>().Where(u => u.IsApproved).Any(EmailPredicate(user.Email));
                    if (exists)
                    {
                        throw new ArgumentException(String.Format("Email address '{0}' already exist", user.Email), "user");
                    }

                    c1User.Email = user.Email;
                }

                c1User.IsApproved = user.IsApproved;
                c1User.LastActivityDate = (user.LastActivityDate == DateTime.MinValue.ToLocalTime()) ? (DateTime?)null : user.LastActivityDate.ToUniversalTime();
                c1User.LastLockoutDate = (user.LastLockoutDate == DateTime.MinValue.ToLocalTime()) ? (DateTime?)null : user.LastLockoutDate.ToUniversalTime();
                c1User.LastLoginDate = (user.LastLoginDate == DateTime.MinValue.ToLocalTime()) ? (DateTime?)null : user.LastLoginDate.ToUniversalTime();
                c1User.LastPasswordChangedDate = (user.LastPasswordChangedDate == DateTime.MinValue.ToLocalTime()) ? (DateTime?)null : user.LastPasswordChangedDate.ToUniversalTime();

                data.Update(c1User);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var data = new DataConnection())
            {
                var user = data.Get<IMembershipUser>().Where(u => u.IsApproved && !u.IsLockedOut).SingleOrDefault(UsernamePredicate(username));
                if (user != null)
                {
                    return PasswordHash.ValidatePassword(password, user.Password);
                }
            }

            return false;
        }

        public static bool SetPassword(string username, string newPassword)
        {
            using (var data = new DataConnection())
            {
                var c1User = data.Get<IMembershipUser>().SingleOrDefault(UsernamePredicate(username));
                if (c1User == null)
                {
                    return false;
                }

                c1User.Password = PasswordHash.HashPassword(newPassword);
                c1User.LastActivityDate = DateTime.UtcNow;
                c1User.LastPasswordChangedDate = DateTime.UtcNow;

                data.Update(c1User);

                return true;
            }
        }

        private MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords, Expression<Func<IMembershipUser, bool>> wherePredicate)
        {
            var coll = new MembershipUserCollection();

            using (var data = new DataConnection())
            {
                var allUsers = data.Get<IMembershipUser>();

                if (wherePredicate != null)
                {
                    allUsers = allUsers.Where(wherePredicate);
                }

                totalRecords = allUsers.Count();

                foreach (var user in allUsers.Skip(pageSize * pageIndex).Take(pageSize))
                {
                    coll.Add(MapUser(user));
                }
            }

            return coll;
        }

        private MembershipUser MapUser(IMembershipUser user)
        {
            return new MembershipUser(
                Name,
                user.UserName,
                user.Id,
                user.Email,
                user.PasswordQuestion,
                user.Comment,
                user.IsApproved,
                user.IsLockedOut,
                user.CreationDate.ToLocalTime(),
                user.LastLoginDate == null ? DateTime.MinValue : user.LastLoginDate.Value.ToLocalTime(),
                user.LastActivityDate == null ? DateTime.MinValue : user.LastActivityDate.Value.ToLocalTime(),
                user.LastPasswordChangedDate == null ? DateTime.MinValue : user.LastPasswordChangedDate.Value.ToLocalTime(),
                user.LastLockoutDate == null ? DateTime.MinValue : user.LastLockoutDate.Value.ToLocalTime());
        }

        private static string GeneratePassword(int passwordLength)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[passwordLength];
            var rd = new Random();

            for (var i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
