using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

internal class TokenManagerService<T> where T : IAuthUser
{
    private readonly ICrudService<T> crudService;
    public TokenManagerService(ICrudService<T> crudService)
    {
        this.crudService = crudService;
    }
    public async Task AddTokenAsync(T user, Token token)
    {
        user.Tokens.Add(token);
        await crudService.ReplaceAsync(user); //TODO: WRITE UPDATE METHOD
    }
    public void AddToken(T user, Token token)
    {
        user.Tokens.Add(token);
        crudService.Replace(user); //TODO: WRITE UPDATE METHOD
    }
    public async Task ClearExpiredTokensAsync(T user)
    {
        var foundUser= (await crudService.FindAsync(z => z._Id == user._Id))
            .FirstOrDefault();
        var ExpiredTokens = foundUser.Tokens
            .RemoveAll(token =>
            DateTime.Compare(token.Expiration, DateTime.Now) <= 0);

        await crudService.ReplaceAsync(user); //TODO: WRITE UPDATE METHOD
    }
    public void ClearExpiredTokens(T user)
    {
        var foundUser = ( crudService.Find(z => z._Id == user._Id))
            .FirstOrDefault();
        var ExpiredTokens = foundUser.Tokens
            .RemoveAll(token =>
            DateTime.Compare(token.Expiration, DateTime.Now) <= 0);

        crudService.Replace(user); //TODO: WRITE UPDATE METHOD
    }
    public async Task<Token> GetAuthTokenAsync(T user, string tokenKey)
    {
        var foundUser = (await crudService.FindAsync(z => z._Id == user._Id))
            .FirstOrDefault();

        Token foundToken = foundUser?.Tokens.
            Where(t => t.Key == tokenKey && 
            DateTime.Compare(t.Expiration,DateTime.Now)<0 )
            .FirstOrDefault();

        if(foundToken != null)
        {
            return foundToken;
        }
        else
        {
            await ClearExpiredTokensAsync(user);
            throw new KeyNotFoundException("Invalid Token Key");
        }
    }
    public void RemoveToken(string tokenKey, int tokenCode = -1)
    {
        T user = GetUserWithTokenKey(tokenKey, tokenCode);

        user.Tokens.RemoveAll(t => t.Key == tokenKey);

        crudService.Replace(user);

    }
    public async Task RemoveTokenAsync(string tokenKey, int tokenCode = -1)
    {
        T user = await GetUserWithTokenKeyAsync(tokenKey, tokenCode);

        user.Tokens.RemoveAll(t => t.Key == tokenKey);

        await crudService.ReplaceAsync(user);

    }
    public async Task<T> GetUserWithTokenKeyAsync(string tokenKey, int tokenCode = -1)
    {
        T user = tokenCode == -1
            ? (await crudService.FindAsync(u => u.Tokens.Any(t => t.Key == tokenKey))).FirstOrDefault()
            : (await crudService.FindAsync(u => u.Tokens.Any(t => t.Key == tokenKey && t.TokenCode == tokenCode))).FirstOrDefault();
        
        return user == null ? throw new Exception("Token not found.") : user;
    }
    public T GetUserWithTokenKey(string tokenKey, int tokenCode = -1)
    {
        T user = tokenCode == -1
            ? crudService.Find(u => u.Tokens.Any(t => t.Key == tokenKey)).FirstOrDefault()
            : crudService.Find(u => u.Tokens.Any(t => t.Key == tokenKey && t.TokenCode == tokenCode)).FirstOrDefault();
        return user == null ? throw new Exception("Token not found.") : user;
    }


}

