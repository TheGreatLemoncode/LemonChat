using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{
    internal class Person
    {
        private Guid UserId;
        private string Name;
        private List<Person> Friends;

        public Person()
        {
            UserId = Guid.NewGuid();
            Friends = new List<Person>();
        }

        public Person(string pName)
        {
            UserId= Guid.NewGuid();
            Name = pName;
            Friends = new List<Person>();
        }

        public bool AddFriend(Person pPerson)
        {
            if (Friends.Contains(pPerson))
            {
                return false;
            }
            Friends.Add(pPerson);
            return true;
        }

        public bool RemoveFriend(Guid PersonId)
        {
            foreach (Person pPerson in Friends)
            {
                if(pPerson.UserId == PersonId)
                {
                    Friends.Remove(pPerson);
                    return true;
                }
            }
            return false;
        }
    }
}
