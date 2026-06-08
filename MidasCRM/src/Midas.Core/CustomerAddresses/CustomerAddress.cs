using System;
using System.Collections.Generic;
using System.Text;
using Midas.Core.Customers;
using Midas.Core.Enums;
using Midas.Core.Orders;

namespace Midas.Core.CustomerAddresses
{
    public class CustomerAddress : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;

        public string? NovaPoshtaCityRef { get; private set; }
        public string? NovaPoshtaWarehouseRef { get; private set; }

        public string City { get; private set; }
        public int PostDepartmentNumber { get; private set; }
        public DeliveryPointType DeliveryPointType { get; private set; }

        public Guid CompanyId { get; private set; }
        public bool IsDeleted { get; private set; }


        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private CustomerAddress(
            int id,
            int customerId,
            string city,
            int postDepartmentNumber,
            DeliveryPointType deliveryPointType,
            Guid companyId)
        {
            Id = id;
            CustomerId = customerId;
            City = city;
            PostDepartmentNumber = postDepartmentNumber;
            DeliveryPointType = deliveryPointType;
            CompanyId = companyId;
        }

        public static CustomerAddress Create(
            int id,
            int customerId,
            string city,
            int postDepartmentNumber,
            DeliveryPointType deliveryPointType,
            Guid companyId)
        {
            return new CustomerAddress(
                id,
                customerId,
                city,
                postDepartmentNumber,
                deliveryPointType,
                companyId);
        }

        public static CustomerAddress Create(
            Customer customer,
            string city,
            int postDepartmentNumber,
            DeliveryPointType deliveryPointType,
            Guid companyId)
        {
            var address = new CustomerAddress(
                0,
                customer.Id,
                city,
                postDepartmentNumber,
                deliveryPointType,
                companyId);

            address.Customer = customer;
            return address;
        }

        public void Update(
            int customerId,
            string city,
            int postDepartmentNumber,
            DeliveryPointType deliveryPointType)
        {
            CustomerId = customerId;
            City = city;
            PostDepartmentNumber = postDepartmentNumber;
            DeliveryPointType = deliveryPointType;
        }

        public void UpdateDelivery(
            string city,
            int postDepartmentNumber,
            DeliveryPointType deliveryPointType)
        {
            City = city;
            PostDepartmentNumber = postDepartmentNumber;
            DeliveryPointType = deliveryPointType;
            NovaPoshtaCityRef = null;
            NovaPoshtaWarehouseRef = null;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public void SetNovaPoshtaRefs(string cityRef, string warehouseRef)
        {
            if (string.IsNullOrWhiteSpace(cityRef))
                throw new ArgumentException("CityRef cannot be empty", nameof(cityRef));
            if (string.IsNullOrWhiteSpace(warehouseRef))
                throw new ArgumentException("WarehouseRef cannot be empty", nameof(warehouseRef));

            NovaPoshtaCityRef = cityRef;
            NovaPoshtaWarehouseRef = warehouseRef;
        }
    }
}

