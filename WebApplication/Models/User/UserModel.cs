using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebApplication.Lib.Dal.DbContext;

namespace WebApplication.Models.User
{
    public class UserModel : ResultModel
    {
        [Display(Name = @"Id")]
        public long Id { get; set; }

        [Display(Name = @"Email")]
        [AllowHtml]
        public string Email { get; set; }

        [Display(Name = @"Password")]
        [AllowHtml]
        public string Password { get; set; }

        [Display(Name = @"Firstname")]
        [AllowHtml]
        public string Firstname { get; set; }

        [Display(Name = @"Lastname")]
        [AllowHtml]
        public string Lastname { get; set; }

        [Display(Name = @"Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        [Display(Name = @"Address")]
        [AllowHtml]
        public string Address { get; set; }

        [Display(Name = @"Gender")]
        public int Gender { get; set; }

        [Display(Name = @"DateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = @"ActiveToken")]
        [AllowHtml]
        public string ActiveToken { get; set; }

        [Display(Name = @"ForgotPasswordToken")]
        [AllowHtml]
        public string ForgotPasswordToken { get; set; }

        [Display(Name = @"TokenCreatedDate")]
        public DateTime TokenCreatedDate { get; set; }

        [Display(Name = @"RoleId")]
        public long RoleId { get; set; }

        [Display(Name = @"Status")]
        public long Status { get; set; }

        [Display(Name = @"Province")]
        [AllowHtml]
        public string Province { get; set; }

        [Display(Name = @"District")]
        [AllowHtml]
        public string District { get; set; }

        [Display(Name = @"Ward")]
        [AllowHtml]
        public string Ward { get; set; }

        public UserModel() { }

        public UserModel(Account account)
        {
            if (account == null)
            {
                return;
            }

            Id = account.Id;
            Firstname = account.Firstname;
            Lastname = account.Lastname;
            Gender = account.Gender;
            ActiveToken = account.ActiveToken;
            Address = account.Address;
            DateOfBirth = account.DateOfBirth ?? DateTime.Now;
            Email = account.Email;
            ForgotPasswordToken = account.ForgotPasswordToken;
            RoleId = account.RoleId;
            Phone = account.Phone;
            Status = account.Status;
            Province = account.Province;
            District = account.District;
            Ward = account.Ward;
        }
    }
}