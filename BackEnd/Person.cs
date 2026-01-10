using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.User
{
    public class Person
    {
        private string Name;
        private List<Person> Contacts;

        public Person()
        {
            Contacts = new List<Person>();
        }

        public Person(string pName)
        {
            Name = pName;
            Contacts = new List<Person>();
        }

        /// <summary>
        /// Allow the creation of a Person instance using a credential instance
        /// </summary>
        /// <param name="pCredential"></param>
        public Person(Security.Credential pCredential)
        {
            Name = pCredential.DisplayName;
            Contacts = new List<Person>();
        }

        public void AddFriends(List<Person> pListe)
        {
            foreach(Person p in pListe)
            {
                this.AddFriend(p);
            }
        }

        public bool AddFriend(Person pPerson)
        {
            if (Contacts.Contains(pPerson))
            {
                return false;
            }
            Contacts.Add(pPerson);
            return true;
        }

        public bool RemoveContact(Person pContact)
        {
            if (!Contacts.Contains(pContact))
            {
                return false;
            }
            Contacts.Remove(pContact);
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
