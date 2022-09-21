using System.Globalization;
using Allure.LightBdd.Examples.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Parameters;
using NUnit.Allure.Attributes;

namespace Allure.LightBdd.Examples.Context
{
    public class ContactsManagementContext
    {
        private ContactBook _contactBook = new ContactBook();
        private readonly List<Contact> _addedContacts = new List<Contact>();
        private readonly List<Contact> _removedContacts = new List<Contact>();
        private Contact[] _searchResults = new Contact[0];

        [AllureStep(nameof(Given_my_contact_book_is_empty))]
        public async Task Given_my_contact_book_is_empty()
        {
            _contactBook = new ContactBook();
            await Task.Delay(200);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {nameof(Given_my_contact_book_is_empty)}");
        }

        [AllureStep(nameof(When_I_add_new_contacts))]
        public async Task When_I_add_new_contacts()
        {
            AddSomeContacts();
            await Task.Delay(200);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {nameof(When_I_add_new_contacts)}");
        }

        [AllureStep(nameof(Then_all_contacts_should_be_available_in_the_contact_book))]
        public async Task Then_all_contacts_should_be_available_in_the_contact_book()
        {
            await Task.Delay(200);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {nameof(Then_all_contacts_should_be_available_in_the_contact_book)}");
            Assert.That(
                _contactBook.Contacts.ToArray(),
                Is.EquivalentTo(_addedContacts),
                "Contacts should be added to contact book");
        }

        [AllureStep(nameof(Given_my_contact_book_is_filled_with_contacts))]
        public async Task  Given_my_contact_book_is_filled_with_contacts()
        {
            _contactBook = new ContactBook();
            AddSomeContacts();
            await Task.Delay(200);
        }

        [AllureStep(nameof(When_I_remove_one_contact))]
        public async Task  When_I_remove_one_contact()
        {
            RemoveContact(_contactBook.Contacts.First());
            await Task.Delay(200);
        }

        [AllureStep(nameof(Then_the_contact_book_should_not_contain_removed_contact_any_more))]
        public async Task  Then_the_contact_book_should_not_contain_removed_contact_any_more()
        {
            await Task.Delay(200);
            Assert.AreEqual(
                Enumerable.Empty<Contact>(),
                _contactBook.Contacts.Where(c => _removedContacts.Contains(c)).ToArray(),
                "Contact book should not contain removed books");
        }

        [AllureStep(nameof(Then_the_contact_book_should_contains_all_other_contacts))]
        public async Task  Then_the_contact_book_should_contains_all_other_contacts()
        {
            await Task.Delay(200);
            Assert.That(
                _addedContacts.Except(_removedContacts).ToArray(),
                Is.EquivalentTo(_contactBook.Contacts.ToArray()),
                "All contacts that has not been explicitly removed should be still present in contact book");
        }

        [AllureStep(nameof(Given_my_contact_book_is_filled_with_many_contacts))]
        public async Task  Given_my_contact_book_is_filled_with_many_contacts()
        {
            await Task.Delay(200);
            for (var i = 0; i < 10000; ++i)
                _contactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        [AllureStep(nameof(When_I_clear_it))]
        public async Task  When_I_clear_it()
        {
            await Task.Delay(200);
            foreach (var contact in _contactBook.Contacts.ToArray())
                RemoveContact(contact);
            StepExecution.Current.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        private void RemoveContact(Contact contact)
        {
            _removedContacts.Add(contact);
            _contactBook.Remove(contact.Email);
        }

        [AllureStep(nameof(Then_the_contact_book_should_be_empty))]
        public async Task  Then_the_contact_book_should_be_empty()
        {
            await Task.Delay(200);
            Assert.IsEmpty(_contactBook.Contacts, "Contact book should be empty");
        }

        private void AddSomeContacts()
        {
            var contacts = new[]
            {
                new Contact("Jack", "123-456-789","justjack@hotmail.com"),
                new Contact("Samantha", "321-654-987","samantha359@gmai.com"),
                new Contact("Josh", "132-465-798","jos4@gmail.com")
            };

            foreach (var contact in contacts)
                AddContact(contact);
        }

        private void AddContact(Contact contact)
        {
            _addedContacts.Add(contact);
            _contactBook.AddContact(contact.Name, contact.PhoneNumber, contact.Email);
        }

        [AllureStep(nameof(When_I_search_for_contacts_by_phone_starting_with))]
        public async Task When_I_search_for_contacts_by_phone_starting_with(string with)
        {
            await Task.Delay(200);
            _searchResults = _contactBook.SearchByPhoneStartingWith(with).ToArray();
        }

        [AllureStep(nameof(Then_I_should_receive_contacts))]
        public async Task Then_I_should_receive_contacts(VerifiableTable<Contact> contacts)
        {
            contacts.SetActual(_searchResults);
            await Task.Delay(200);
        }

        [AllureStep(nameof(Given_I_added_contacts))]
        public async Task Given_I_added_contacts(InputTable<Contact> contacts)
        {
            await Task.Delay(200);
            foreach (var contact in contacts)
                AddContact(contact);
        }

        [AllureStep(nameof(When_I_request_contacts_sorted_by_name))]
        public async Task When_I_request_contacts_sorted_by_name()
        {
            await Task.Delay(200);
            _searchResults = _contactBook.GetNameSortedContacts().ToArray();
        }
    }
}