using AuthAuthEasyLib.Common;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {


        public async Task RegisterAsync(T newUser, TimeSpan? verificationTokenSpan = null)
        {


            if (crudType == CrudType.Sql)
            {
                var uniqueProps = typeof(T).GetProperties()
             .Where(prop => Attribute.IsDefined(prop, typeof(Attributes.UniqueAttribute)));
                foreach (var prop in uniqueProps)
                {
                    var param = Expression.Parameter(typeof(T));
                    var condition = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(
                            Expression.Property(param, prop.Name),
                            Expression.Constant(prop.GetValue(newUser), prop.PropertyType)
                        ),
                        param
                    ).Compile();
                    if (crudService.FindQueriable().Any(condition))
                        throw new ArgumentException($"{prop.Name} already in use.");
                }
            }

            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = TokenBuilder.GenerateVerificationToken(newUser, verificationTokenSpan ?? TimeSpan.FromDays(7)); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            await crudService.AddAsync(newUser);

        }
        public void Register(T newUser, TimeSpan? verificationTokenSpan = null)
        {
            if (crudType == CrudType.Sql)
            {
                var uniqueProps = typeof(T).GetProperties()
             .Where(prop => Attribute.IsDefined(prop, typeof(Attributes.UniqueAttribute)));
                foreach (var prop in uniqueProps)
                {
                    var param = Expression.Parameter(typeof(T));
                    var condition = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(
                            Expression.Property(param, prop.Name),
                            Expression.Constant(prop.GetValue(newUser), prop.PropertyType)
                        ),
                        param
                    ).Compile();
                    if (crudService.FindQueriable().Any(condition))
                        throw new ArgumentException($"{prop.Name} already in use.");
                }
            }

            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = TokenBuilder.GenerateVerificationToken(newUser, verificationTokenSpan ?? TimeSpan.FromDays(7)); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            crudService.Add(newUser);
        }
    }
}
