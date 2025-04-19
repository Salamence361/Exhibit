﻿using Microsoft.AspNetCore.Identity;
namespace fly.Models;
public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"Имя пользователя '{userName}' уже занято."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"Email '{email}' уже используется."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Пароль должен быть не менее {length} символов."
        };
    }

    // Добавьте другие методы для локализации других сообщений об ошибках
}