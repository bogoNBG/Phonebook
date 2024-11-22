using System.Collections.Generic;

namespace WpfApp4.Model
{
    internal class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }       
        public string Number { get; set; }
        public string Email { get; set; }
        public List<Link> Links { get; set; }

        public Contact(int id, string name, string number, string email)
        {
            this.Id = id;
            this.Name = name;
            this.Number = number;
            this.Email = email;
            this.Links = new List<Link>();
        }

        public Contact(string name, string number, string email)
        {
            this.Name = name;
            this.Number = number;
            this.Email = email;
            this.Links = new List<Link>();
        }
    }
}