using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Security;

using Composite;
using Composite.Data;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Web
{
    public class CompositeC1MembershipProvider : MembershipProvider
    {
        private static readonly Func<string, Expression<Func<IMembershipUser, bool>>> UsernamePredicate = userName => (u => String.Compare(u.UserName, userName, StringComparison.OrdinalIgnoreCase) == 0);
        private static readonly Func<string, Expression<Func<IMembershipUser, bool>>> EmailPredicate = email => (u => String.Compare(u.Email, email, StringComparison.OrdinalIgnoreCase) == 0);

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts => int.MaxValue;

        public override int MinRequiredNonAlphanumericCharacters => throw new NotSupportedException();

        public override int MinRequiredPasswordLength => 6;

        public override int PasswordAttemptWindow => throw new NotSupportedException();

        public override MembershipPasswordFormat PasswordFormat => MembershipPasswordFormat.Hashed;

        public override string PasswordStrengthRegularExpression => throw new NotSupportedException();

        public override bool RequiresQuestionAndAnswer => false;

        public override bool RequiresUniqueEmail => true;

        public override bool EnablePasswordReset => true;

        public override bool EnablePasswordRetrieval => false;

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

            if (!MailAddressValidator.IsValid(email))
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

                var id = (Guid?)providerUserKey ?? Guid.NewGuid();

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
                throw new ArgumentException("UserKey has to be of type Guid", nameof(providerUserKey));
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
            Verify.ArgumentNotNullOrEmpty(user.UserName, nameof(user));
            Verify.ArgumentNotNullOrEmpty(user.Email, nameof(user));

            using (var data = new DataConnection())
            {
                var c1User = data.Get<IMembershipUser>().SingleOrDefault(u => u.Id == (Guid)user.ProviderUserKey);
                if (c1User == null)
                {
                    throw new ArgumentException($"Provider user key '{user.ProviderUserKey}' doesn't exist", nameof(user));
                }

                if (!user.Email.Equals(c1User.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = data.Get<IMembershipUser>().Where(u => u.IsApproved).Any(EmailPredicate(user.Email));
                    if (exists)
                    {
                        throw new ArgumentException($"Email address '{user.Email}' already exist", nameof(user));
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
                user.LastLoginDate?.ToLocalTime() ?? DateTime.MinValue,
                user.LastActivityDate?.ToLocalTime() ?? DateTime.MinValue,
                user.LastPasswordChangedDate?.ToLocalTime() ?? DateTime.MinValue,
                user.LastLockoutDate?.ToLocalTime() ?? DateTime.MinValue);
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
