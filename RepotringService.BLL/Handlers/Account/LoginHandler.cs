﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReportingService.BLL;
using ReportingService.BLL.Errors;
using ReportingService.Models;
using RepotringService.BLL.Commands.Account;
using RepotringService.BLL.Responses.Account;

namespace IntershipProject.BLL.Handlers.Account
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<UserModel, Error>>
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public LoginHandler(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<Result<UserModel, Error>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
            if (user == null)
                return Result<UserModel, Error>.Failed(new NotFoundError("This account doesn't exist"));

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Result<UserModel, Error>.Failed(new BadRequestError("Wrong password"));

            var userModel = new UserModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            return Result<UserModel, Error>.Succeeded(userModel);
        }
    }
}
