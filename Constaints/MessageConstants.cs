using System.Data.Common;

namespace QuanLySoTietKiem.Constaints
{
    public static class MessageConstants
    {
        public static class Success
        {
            public const string CreateAccount = "Tạo tài khoản thành công";
            public const string UpdateAvatar = "Cập nhật ảnh đại diện thành công";
            public const string WithdrawMoney = "Rút tiền thành công";
            public const string AddMoney = "Nạp tiền thành công";
        }

        public static class Error
        {
            public const string InsufficientBalance = "Số dư không đủ";
            public const string AccountNotFound = "Không tìm thấy tài khoản";
            public const string InvalidAmount = "Số tiền không hợp lệ";
        }

        public static class Warning
        {
            public const string WithdrawBeforeMaturity = "Rút tiền trước hạn sẽ được áp dụng lãi suất thấp hơn";
            public const string NotDueDate = "Chưa tới ngày đáo hạn để nạp thêm tiền";
        }

        public static class Login
        {
            public const string AccountNotFound = "Tài khoản không tồn tại";
            public const string EmailNotConfirmed = "Email chưa được xác thực vui lòng kiểm tra email";
            public const string AccountLocked = "Tài khoản đã bị khóa do nhiều lần đăng nhập sai";
            public const string InvalidPassword = "Sai mật khẩu";
            public const string LoginSuccess = "Đăng nhập thành công";
            public const string LoginFailed = "Đăng nhập thất bại";
            public const string AccountLockedOut = "Tài khoản đã bị khóa do nhiều lần đăng nhập sai";
            public const string ErrorDuringPassword = "Có lỗi xảy ra trong quá trình đăng nhập";
        }

        public static class LoginModel
        {
            // Username field
            public const string UserNameRequired = "Username is required";
            public const string UserNameMinLength = "Username must be at least 3 characters long";
            public const string UserNameMaxLength = "Username must be less than 50 characters long";

            // Password field
            public const string PasswordRequired = "Password is required";
            public const string PasswordMinLength = "Password must be at least 8 characters long";
            public const string PasswordMaxLength = "Password must be less than 50 characters long";
        }
        public static class ModelRegister
        {
            // Username field
            public const string UserNameRequired = "Username is required";
            public const string UserNameMinLength = "Username must be at least 3 characters long";
            public const string UserNameMaxLength = "Username must be less than 50 characters long";
            public const string UserNameExists = "Username already exists in the system";

            // Full name field
            public const string FullNameRequired = "Full name is required";
            public const string FullNameMinLength = "Full name must be at least 3 characters long";
            public const string FullNameMaxLength = "Full name must be less than 50 characters long";

            // Email field
            public const string EmailRequired = "Email is required";
            public const string EmailFormat = "Invalid email format";
            public const string EmailExists = "This email is already registered in the system";

            // Password field
            public const string PasswordRequired = "Password is required";
            public const string PasswordMinLength = "Password must be at least 6 characters long";
            public const string PasswordMaxLength = "Password must be less than 30 characters long";
            public const string PasswordRequiresDigit = "Password must contain at least one digit";
            public const string PasswordRequiresLower = "Password must contain at least one lowercase letter";
            public const string PasswordRequiresUpper = "Password must contain at least one uppercase letter";
            public const string PasswordRequiresNonAlphanumeric = "Password must contain at least one special character";

            // Address field
            public const string AddressRequired = "Address is required";
            public const string AddressMinLength = "Address must be at least 3 characters long";
            public const string AddressMaxLength = "Address must be less than 255 characters long";
            public const string AddressInvalid = "Invalid address, please check again";

            // Citizen Identification field
            public const string CitizenIdentificationRequired = "Citizen identification number is required";
            public const string CitizenIdentificationMinLength = "Citizen identification number must be at least 9 characters long";
            public const string CitizenIdentificationMaxLength = "Citizen identification number must be less than 12 characters long";
            public const string CitizenIdentificationExists = "This citizen identification number is already registered";
            public const string CitizenIdentificationInvalid = "Invalid citizen identification number";

            // Confirm password field
            public const string ConfirmPasswordRequired = "Confirm password is required";
            public const string ConfirmPasswordMinLength = "Confirm password must be at least 6 characters long";
            public const string ConfirmPasswordMaxLength = "Confirm password must be less than 30 characters long";
            public const string ComparePassword = "Password and confirmation password do not match";

            // Phone number field
            public const string PhoneNumberRequired = "Phone number is required";
            public const string PhoneNumberMinLength = "Phone number must be at least 10 digits";
            public const string PhoneNumberMaxLength = "Phone number must be less than 11 digits";
            public const string PhoneNumberInvalid = "Invalid phone number";
            public const string PhoneNumberExists = "This phone number is already registered";

            // Default account balance
            public const int AccountBalance = 100000000;

            // Registration messages
            public const string RegisterSuccess =
                "Account registration successful! Please check your email to confirm your account.";

            public const string RegisterFailed =
                "Account registration failed. Please try again later.";

            public const string EmailConfirmationSent =
                "Confirmation email has been sent. Please check your inbox.";

            public const string EmailConfirmationSuccess =
                "Email confirmed successfully! You can now log in.";

            public const string EmailConfirmationFailed =
                "Email confirmation failed. Please try again.";
        }

        public static class ResetPasswordModel
        {
            public const string MinLengthPassword = "Mật khẩu phải có ít nhất 8 ký tự";
            public const string PasswordNotMatch = "Mật khẩu không khớp";
        }

        public static class EditProfileModel
        {
            public const string FullNameRequired = "Vui lòng nhập họ tên";
            public const string FullNameMaxStringLength = "Họ tên không được vượt quá 50 ký tự";

            public const string EmailRequired = "Vui lòng nhập địa chỉ email";
            public const string EmailFormat = "Email chưa đúng định dạng";


            //Trường địa chỉ 
            public const string AddressRequired = "Vui lòng nhập địa chỉ";
            public const string AddressMinLength = "Địa chỉ phải có ít nhất 3 ký tự";
            public const string AddressMaxLength = "Địa chỉ phải nhỏ hơn 255 ký tự";


            //Trường số điện thoại 
            public const string PhoneNumberRequired = "Vui lòng nhập số điện thoại";
            public const string PhoneNumberMinLength = "Số điện thoại phải có ít nhất 10 ký tự";
            public const string PhoneNumberMaxLength = "Số điện thoại phải nhỏ hơn 11 ký tự";
        }

        public static class UpdateAvatarModel
        {
            public const string AvatarRequired = "Vui lòng chọn ảnh";
        }
    }
}