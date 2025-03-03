using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;
using Tracker.Core.Api.Tests.Acceptance.Models.Transactions;

namespace Tracker.Core.Api.Tests.Acceptance.Models.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
