# Auth Auth Easy
.NET Standart library that handles Authorization/Authentication methods by using NoSQL (MongoDB) database.

# Usage
#### Creating User Model
```C#
using AuthAuthEasyLib.Bases;

public class UserModel : AuthUser // Must use AuthUser class as base class
{
 // Add your additional properties that does not included in AuthUser class.
}
```

#### Instantiating AuthService
```C#
using AuthAuthEasyLib.Services; // Add using statement to access services namespace

 var mongoConfig = new MongoCrudServiceConfig(
                "CONNECTION STRING",
                "DATABASE NAME",
                "COLECTION NAME");
// or

MongoClientSettings settings = new MongoClientSettings(); // Then configure settings
var mongoConfig = new MongoCrudServiceConfig(
                settings,
                "DATABASE NAME",
                "COLECTION NAME");

var authService = new AuthService<UserModel>(mongoConfig); // Instantiate new Auth Service with the generic type of your UserModel.
//Note: UserModel class must use AuthUser class as base.

```
### Methods ( Async methods are also available. )
#### Register
```C#
var newUser = new UserModel() {Username = "USERNAME", Password = "PASSWORD"};
authService.Register(newUser);
```


##### Login
```C#

var (loggedInUser, authToken) = authService.LoginWithUsername("USERNAME", "PASSWORD"); // Returns a tuple of UserModel and Token
// or
var (loggedInUser, authToken) = authService.Login(user => 
                                      user.PhoneNumber == "PHONE NUMBER", 
                                      "PASSWORD"); // You can log in with any identifier. // Returns a tuple of UserModel and Token
```

#### Verification
```C#
                                      
// Creates or gets  verification token for user with the expiration time provided.
var verificationToken = authService.GetOrCreateVerificationToken("USER ID", new Timespan(7,0,0,0)) 
// Verifies User
authService.VerifyUser(verificationToken.Key);

```

#### Logout
```C#
authService.Logout(authToken.Key);

// Logout all sessions:
var user = authService.GetUserById("USER ID");
foreach (var token in user.Tokens.Where(t => t.TokenCode == 1)) // TokenCode 1 for auth Tokens
{
    authService.LogOut(token.Key);
}
```


#### IsAuthorized
```C#
var (isAuthorized, Except, User) = authService.IsAuthorized("AUTH TOKEN KEY"); 
// or 
var (isAuthorized, Except, User) = authService.IsAuthorized("AUTH TOKEN KEY","admin",true); // Requires admin role case insensitive
if(isAuthorized)
{
  // User Is Authorized
}else
{
  throw Except;
}
```


#### Change Password
```C#
authService.ChangePasswordAsync("USER ID", "OLD PASSWORD", "NEW PASSWORD");
//or
authService.ChangePasswordAsync(user =>
                                user.Username == "USERNAME",
                                "OLD PASSWORD", 
                                "NEW PASSWORD");
```



#### Reset Password
```C#
var passwordResetToken = authService.AddPaswordResetRequest(user => 
                                                            user._Id == "USER ID",
                                                            new TimeSpan(7,0,0,0)); // Adds Password Reset Token to found user
authService.ResetPassword(passwordResetToken.Key, "NEW PASSWORD");              
```

#### Update User Info
```C#
authService.UpdateUser("USER ID", user => user.Username = "NEW USERNAME"); 
```
