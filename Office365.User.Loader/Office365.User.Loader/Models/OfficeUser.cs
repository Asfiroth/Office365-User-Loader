﻿namespace Office365.User.Loader.Models
{
    public class OfficeUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ShowOffName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public string License { get; set; }
        public string UsageLocation { get; set; }
        public OfficeUserStatus Status { get; set; }
    }

    public enum OfficeUserStatus
    {
        NotLoaded = 0,
        Loading = 1,
        Loaded = 2
    }
}
