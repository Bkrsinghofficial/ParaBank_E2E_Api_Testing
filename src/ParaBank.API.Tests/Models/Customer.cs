using Newtonsoft.Json;

namespace ParaBank.API.Tests.Models
{
    /// <summary>
    /// Represents a ParaBank customer returned by the login endpoint.
    /// OOP: Encapsulation — properties expose data via public accessors backed by private setters.
    /// </summary>
    public sealed class Customer
    {
        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("firstName")]
        public string FirstName { get; private set; } = string.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; private set; } = string.Empty;

        [JsonProperty("address")]
        public Address? Address { get; private set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; private set; } = string.Empty;

        [JsonProperty("ssn")]
        public string Ssn { get; private set; } = string.Empty;

        [JsonProperty("username")]
        public string Username { get; private set; } = string.Empty;

        [JsonProperty("password")]
        public string Password { get; private set; } = string.Empty;

        // Parameterless constructor for JSON deserialization
        [JsonConstructor]
        public Customer() { }

        /// <summary>Returns a display-friendly summary of the customer.</summary>
        public override string ToString() => $"Customer[Id={Id}, Name={FirstName} {LastName}]";
    }

    /// <summary>
    /// Represents a physical address embedded in the Customer model.
    /// OOP: Composition — Customer "has-a" Address.
    /// </summary>
    public sealed class Address
    {
        [JsonProperty("street")]
        public string Street { get; private set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; private set; } = string.Empty;

        [JsonProperty("state")]
        public string State { get; private set; } = string.Empty;

        [JsonProperty("zipCode")]
        public string ZipCode { get; private set; } = string.Empty;

        [JsonConstructor]
        public Address() { }

        public override string ToString() => $"{Street}, {City}, {State} {ZipCode}";
    }
}
