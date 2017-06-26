using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web.Mvc;
using WebApplication.Lib.Dal.DbContext;

namespace WebApplication.Models.User
{
    public class AccountViewModel : ISerializable
    {
        public long Id { get; set; }

        [AllowHtml]
        public string FirstName { get; set; }

        [AllowHtml]
        public string LastName { get; set; }

        [AllowHtml]
        public string Email { get; set; }

        public int Gender { get; set; }

        [AllowHtml]
        public string Phone { get; set; }

        public int Status { get; set; }

        public AccountViewModel() { }

        public AccountViewModel(Account account)
        {
            if (account == null) return;

            Id = account.Id;
            FirstName = account.Firstname;
            LastName = account.Lastname;
            Email = account.Email;
            Gender = account.Gender;
            Phone = account.Phone;
            Status = account.Status;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("FirstName", FirstName);
            info.AddValue("LastName", LastName);
            info.AddValue("Email", Email);
            info.AddValue("Gender", Gender);
            info.AddValue("Phone", Phone);
            info.AddValue("Status", Status);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            GetObjectData(info, context);
        }
    }
}