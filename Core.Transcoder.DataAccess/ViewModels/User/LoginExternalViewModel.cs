using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels.User
{
    public class LoginExternalViewModel
    {
        public enum Provider { Facebook, Google }

        public Provider ProviderType { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
