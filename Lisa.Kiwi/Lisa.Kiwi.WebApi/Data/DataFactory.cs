﻿using System;
using Newtonsoft.Json.Linq;

namespace Lisa.Kiwi.WebApi
{
    internal class DataFactory
    {
        public ReportData Create(JToken json)
        {
            var reportData = new ReportData();
            Modify(reportData, json);
            return reportData;
        }

        public void Modify(ReportData reportData, JToken json)
        {
            reportData.Category = json.Value<string>("category") ?? reportData.Category;

            reportData.IsVisible = json.Value<bool?>("isVisible") ?? reportData.IsVisible;
            
            var statusString = json["status"] != null ? json.Value<string>("status") : null;
            reportData.Status = statusString != null ? (StatusEnum)Enum.Parse(typeof(StatusEnum), statusString, true) : reportData.Status;

            reportData.Description = json.Value<string>("description") ?? reportData.Description;

            reportData.IsUnconscious = json.Value<bool?>("isUnconscious") ?? reportData.IsUnconscious;

            reportData.StolenObject = json.Value<string>("stolenObject") ?? reportData.StolenObject;
            reportData.DateOfTheft = json.Value<DateTime?>("dateOfTheft") ?? reportData.DateOfTheft;

            reportData.DrugsAction = json.Value<string>("drugsAction") ?? reportData.DrugsAction;
            
            reportData.FighterCount = json.Value<int?>("fighterCount") ?? reportData.FighterCount;
            reportData.IsWeaponPresent = json.Value<bool?>("isWeaponPresent") ?? reportData.IsWeaponPresent;

            reportData.WeaponType = json.Value<string>("weaponType") ?? reportData.WeaponType;
            reportData.WeaponLocation = json.Value<string>("weaponLocation") ?? reportData.WeaponLocation;

            reportData.Victim = json.Value<string>("victim") ?? reportData.Victim;
            reportData.VictimName = json.Value<string>("victimName") ?? reportData.VictimName;


            if (json["location"] != null)
            {
                reportData.Location = Modify(reportData.Location, json["location"]);
            }
            if (json["contact"] != null)
            {
                reportData.Contact = Modify(reportData.Contact, json["contact"]);
            }
            if (json["perpetrator"] != null)
            {
                reportData.Perpetrator = Modify(reportData.Perpetrator, json["perpetrator"]);
            }
            if (json["vehicle"] != null)
            {
                reportData.Vehicle = Modify(reportData.Vehicle, json["vehicle"]);
            }
        }

        public VehicleData Create(Vehicle vehicle)
        {
            var vehicleData = new VehicleData
            {
                Brand = vehicle.Brand,
                Color = vehicle.Color,
                NumberPlate = vehicle.NumberPlate,
                AdditionalFeatures = vehicle.AdditionalFeatures
            };

            return vehicleData;
        }

        public ContactData Create(Contact contact)
        {
            var contactData = new ContactData
            {
                EmailAddress = contact.EmailAddress,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber
            };

            return contactData;
        }

        private LocationData Modify(LocationData locationData, JToken json)
        {
            var data = locationData ?? new LocationData();
            data.Building = json["building"] != null ? json.Value<string>("building") : data.Building;
            data.Description = json["description"] != null ? json.Value<string>("description") : data.Description;
            return data;
        }

        private ContactData Modify(ContactData contactData, JToken json)
        {
            var data = contactData ?? new ContactData();
            data.Name = json["name"] != null ? json.Value<string>("name") : data.Name;
            data.PhoneNumber = json["phoneNumber"] != null ? json.Value<string>("phoneNumber") : data.PhoneNumber;
            data.EmailAddress = json["emailAddress"] != null ? json.Value<string>("emailAddress") : data.EmailAddress;
            return data;
        }

        private PerpetratorData Modify(PerpetratorData perpetratorData, JToken json)
        {
            var data = perpetratorData ?? new PerpetratorData();
            data.Name = json["name"] != null ? json.Value<string>("name") : data.Name;

            var sexString = json["sex"] != null ? json.Value<string>("sex") : null;
            data.Sex = sexString != null ? (SexEnum) Enum.Parse(typeof(SexEnum), sexString, true) : SexEnum.Unknown;

            var skinColorString = json["skinColor"] != null ? json.Value<string>("skinColor") : null;
            data.SkinColor = skinColorString != null ? (SkinColorEnum)Enum.Parse(typeof(SkinColorEnum), skinColorString, true) : SkinColorEnum.Unknown;

            data.Clothing = json["clothing"] != null ? json.Value<string>("clothing") : data.Clothing;
            data.MinimumAge = json["minimumAge"] != null ? json.Value<int>("minimumAge") : data.MinimumAge;
            data.MaximumAge = json["maximumAge"] != null ? json.Value<int>("maximumAge") : data.MaximumAge;
            data.UniqueProperties = json["uniqueProperties"] != null ? json.Value<string>("uniqueProperties") : data.UniqueProperties;
            return data;
        }

        private VehicleData Modify(VehicleData vehicleData, JToken json)
        {
            var data = vehicleData ?? new VehicleData();
            data.AdditionalFeatures = json["additionalFeatures"] != null ? json.Value<string>("additionalFeatures") : data.AdditionalFeatures;
            data.Brand = json["brand"] != null ? json.Value<string>("brand") : data.Brand;
            data.Color = json["color"] != null ? json.Value<string>("color") : data.Color;
            data.NumberPlate = json["numberPlate"] != null ? json.Value<string>("numberPlate") : data.NumberPlate;
            return data;
        }
    }
}