using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        public RegisterViewModel(CommunityWebAPIProxy proxy)
        {
            this.proxy = proxy;
            RegisterCommand = new Command(OnRegister);
            CancelCommand = new Command(OnCancel);
            ShowPasswordCommand = new Command(OnShowPassword);
            UploadPhotoCommand = new Command(OnUploadPhoto);
            PhotoURL = proxy.GetDefaultProfilePhotoUrl();
            LocalPhotoPath = "";
            IsPassword = true;
            NameError = "Name is required";
            EmailError = "Email is required";
            PasswordError = "Password must be at least 4 characters long and contain letters and numbers";
        }

        //Defiine properties for each field in the registration form including error messages and validation logic
        #region Name
        private bool showNameError;

        public bool ShowNameError
        {
            get => showNameError;
            set
            {
                showNameError = value;
                OnPropertyChanged("ShowNameError");
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                ValidateName();
                OnPropertyChanged("Name");
            }
        }

        private string nameError;

        public string NameError
        {
            get => nameError;
            set
            {
                nameError = value;
                OnPropertyChanged("NameError");
            }
        }

        private void ValidateName()
        {
            this.ShowNameError = string.IsNullOrEmpty(Name);
        }
        #endregion
        #region Email
        private bool showEmailError;

        public bool ShowEmailError
        {
            get => showEmailError;
            set
            {
                showEmailError = value;
                OnPropertyChanged("ShowEmailError");
            }
        }

        private string email;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                ValidateEmail();
                OnPropertyChanged("Email");
            }
        }

        private string emailError;

        public string EmailError
        {
            get => emailError;
            set
            {
                emailError = value;
                OnPropertyChanged("EmailError");
            }
        }

        private void ValidateEmail()
        {
            this.ShowEmailError = string.IsNullOrEmpty(Email);
            if (!ShowEmailError)
            {
                //check if email is in the correct format using regular expression
                if (!System.Text.RegularExpressions.Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    EmailError = "Email is not valid";
                    ShowEmailError = true;
                }
                else
                {
                    EmailError = "";
                    ShowEmailError = false;
                }
            }
            else
            {
                EmailError = "Email is required";
            }
        }
        #endregion
        #region Password
        private bool showPasswordError;

        public bool ShowPasswordError
        {
            get => showPasswordError;
            set
            {
                showPasswordError = value;
                OnPropertyChanged("ShowPasswordError");
            }
        }

        private string password;

        public string Password
        {
            get => password;
            set
            {
                password = value;
                ValidatePassword();
                OnPropertyChanged("Password");
            }
        }

        private string passwordError;

        public string PasswordError
        {
            get => passwordError;
            set
            {
                passwordError = value;
                OnPropertyChanged("PasswordError");
            }
        }

        private void ValidatePassword()
        {
            //Password must include characters and numbers and be longer than 4 characters
            if (string.IsNullOrEmpty(password) ||
                password.Length < 4 ||
                !password.Any(char.IsDigit) ||
                !password.Any(char.IsLetter))
            {
                this.ShowPasswordError = true;
            }
            else
                this.ShowPasswordError = false;
        }

        //This property will indicate if the password entry is a password
        private bool isPassword = true;
        public bool IsPassword
        {
            get => isPassword;
            set
            {
                isPassword = value;
                OnPropertyChanged("IsPassword");
            }
        }
        //This command will trigger on pressing the password eye icon
        public Command ShowPasswordCommand { get; }
        //This method will be called when the password eye icon is pressed
        public void OnShowPassword()
        {
            //Toggle the password visibility
            IsPassword = !IsPassword;
        }
        #endregion
        #region PhoneNumber
        private string phoneNum;

        public string PhoneNum
        {
            get => phoneNum;
            set
            {
                phoneNum = value;
                ValidatePhone();
                OnPropertyChanged("PhoneNum");
            }
        }
        private bool phoneNumInvalid;

        public bool PhoneNumInvalid
        {
            get => phoneNumInvalid;
            set
            {
                phoneNumInvalid = value;
                OnPropertyChanged("PhoneNumInvalid");
            }
        }
        private void ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(PhoneNum))
            {
                PhoneNumInvalid = true;
                return;
            }

            // Remove common formatting characters
            string cleanedNumber = Regex.Replace(PhoneNum, @"[\s\-\(\)]", "");

            // Regex pattern to match valid phone numbers (international and local)
            string pattern = @"^\+?\d{7,15}$";

            PhoneNumInvalid = !Regex.IsMatch(cleanedNumber, pattern);
        }
        #endregion
        #region Photo

        private string photoURL;

        public string PhotoURL
        {
            get => photoURL;
            set
            {
                photoURL = value;
                OnPropertyChanged("PhotoURL");
            }
        }

        private string localPhotoPath;

        public string LocalPhotoPath
        {
            get => localPhotoPath;
            set
            {
                localPhotoPath = value;
                OnPropertyChanged("LocalPhotoPath");
            }
        }

        public Command UploadPhotoCommand { get; }
        //This method open the file picker to select a photo
        private async void OnUploadPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Please select a photo",
                });

                if (result != null)
                {
                    // The user picked a file
                    this.LocalPhotoPath = result.FullPath;
                    this.PhotoURL = result.FullPath;
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void UpdatePhotoURL(string virtualPath)
        {
            Random r = new Random();
            PhotoURL = proxy.GetImagesBaseAddress() + virtualPath + "?v=" + r.Next();
            LocalPhotoPath = "";
        }

        #endregion

        //Define a command for the register button
        public Command RegisterCommand { get; }
        public Command CancelCommand { get; }

        //Define a method that will be called when the register button is clicked
        public async void OnRegister()
        {
            ValidateName();
            ValidateEmail();
            ValidatePassword();
            ValidatePhone();

            if (!ShowNameError && !ShowEmailError && !ShowPasswordError && !PhoneNumInvalid)
            {
                //Create a new AppUser object with the data from the registration form
                var newAccount = new Account
                {
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    PhoneNumber = PhoneNum
                };

                //Call the Register method on the proxy to register the new user
                InServerCall = true;
                newAccount = await proxy.Register(newAccount);

                //If the registration was successful, navigate to the login page
                if (newAccount != null)
                {
                    //Upload profile image if needed
                    if (!string.IsNullOrEmpty(LocalPhotoPath))
                    {
                        // Login first to establish session
                        var loginResult = await proxy.LoginAsync(new LoginInfo { Email = newAccount.Email, Password = newAccount.Password });
                        if (loginResult != null)
                        {
                            Account? updatedUser = await proxy.UploadProfileImage(LocalPhotoPath);
                            if (updatedUser == null)
                            {
                                await Application.Current.MainPage.DisplayAlert("Registration", "User registered successfully but profile image upload failed.", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Registration", "Registration and profile image upload successful!", "OK");
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Registration", "User registered but couldn't login for image upload. Please try logging in manually.", "OK");
                        }
                    }

                    InServerCall = false;
                    ((App)(Application.Current)).MainPage.Navigation.PopAsync();
                }
                else
                {
                    InServerCall = false;
                    //If the registration failed, display an error message
                    string errorMsg = "Registration failed. Please try again.";
                    await Application.Current.MainPage.DisplayAlert("Registration", errorMsg, "OK");
                }
            }
        }

        //Define a method that will be called upon pressing the cancel button
        public void OnCancel()
        {
            //Navigate back to the login page
            ((App)(Application.Current)).MainPage.Navigation.PopAsync();
        }

    }
}
