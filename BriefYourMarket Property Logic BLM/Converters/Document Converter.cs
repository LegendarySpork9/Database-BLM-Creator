namespace BriefYourMarketPropertyLogicBLM.Converters
{
    internal class DocumentConverter
    {
        public string GetDatabaseField(string field)
        {
            return field switch
            {
                "ADDRESS_1" => "Address1",
                "ADDRESS_2" => "Address2",
                "ADDRESS_3" => "Address3",
                "TOWN" => "Address4",
                "POSTCODE1" => "Postcode1",
                "POSTCODE2" => "Postcode2",
                "FEATURE1" => "ParkingOptions",
                "SUMMARY" => "Summary",
                "DESCRIPTION" => "Description",
                "BRANCH_ID" => "BranchId",
                "BEDROOMS" => "Bedrooms",
                "BATHROOMS" => "Bathrooms",
                "PRICE" => "Price",
                "PRICE_QUALIFIER" => "PriceQualifier",
                "CREATE_DATE" => "DateCreated",
                "UPDATE_DATE" => "UpdateDate",
                "DISPLAY_ADDRESS" => "Name",
                "LIVING_ROOMS" => "ReceptionRooms",
                "PROP_SUB_ID" => "Type",
                "ORIGINAL_ID" => "Id",
                "STATUS_ID" => "NotMapped",
                "PUBLISHED_FLAG" => "NotMapped",
                "LET_BOND" => "NotMapped",
                "LET_DATE_AVAILABLE" => "NotMapped",
                "TRANS_TYPE_ID" => "NotMapped",
                "MIN_SIZE_ENTERED" => "NotMapped",
                "MAX_SIZE_ENTERED" => "NotMapped",
                "PRICE_PER_UNIT" => "NotMapped",
                _ => ""
            };
        }

        public string CheckFieldValue(string field, string value = "")
        {
            return field switch
            {
                "PRICE_QUALIFIER" => GetPriceQualifier(value).ToString(),
                "STATUS_ID" => StandardValues.PropertyValues.StatusId.ToString(),
                "PUBLISHED_FLAG" => StandardValues.PropertyValues.Published.ToString(),
                "PROP_SUB_ID" => GetPropSubId(value).ToString(),
                "LET_BOND" => StandardValues.PropertyValues.LetBond.ToString(),
                "LET_DATE_AVAILABLE" => StandardValues.PropertyValues.LetAvailableDate,
                "TRANS_TYPE_ID" => StandardValues.PropertyValues.TransType.ToString(),
                "MIN_SIZE_ENTERED" => StandardValues.PropertyValues.SizeEentered.ToString(),
                "MAX_SIZE_ENTERED" => StandardValues.PropertyValues.SizeEentered.ToString(),
                "PRICE_PER_UNIT" => StandardValues.PropertyValues.PricePerUnit.ToString(),
                _ => value
            };
        }

        private int GetPriceQualifier(string value)
        {
            return value switch
            {
                "Guide Price" => 2,
                "OffersInExcess" => 4,
                "Offers In Region" => 5,
                "Offers Over" => 7,
                _ => 0
            };
        }

        private int GetPropSubId(string value)
        {
            int propSubId = 0;

            string type = value.Substring(0, value.IndexOf("|"));
            string style = value.Substring(value.IndexOf("|") + 1);

            if (type.Contains(","))
            {
                type = type.Substring(0, type.IndexOf(","));
            }

            if (style.Contains(","))
            {
                style = style.Substring(0, style.IndexOf(","));
            }

            if (type == "Apartment")
            {
                if (style == "Ground Floor")
                {
                    propSubId = 7;
                }
            }

            if (type == "Bungalow")
            {
                if (style == "Detached")
                {
                    propSubId = 15;
                }

                else if (style == "SemiDetached")
                {
                    propSubId = 14;
                }

                else if (style == "Terraced")
                {
                    propSubId = 13;
                }

                else
                {
                    propSubId = 12;
                }
            }

            if (type == "Cottage")
            {
                propSubId = 23;
            }

            if (type == "House")
            {
                if (style == "Detached")
                {
                    propSubId = 4;
                }

                else if (style == "End Terrace")
                {
                    propSubId = 2;
                }

                else if (style == "LinkDetached")
                {
                    propSubId = 21;
                }

                else if (style == "SemiDetached")
                {
                    propSubId = 3;
                }

                else if (style == "Terraced")
                {
                    propSubId = 1;
                }

                else
                {
                    propSubId = 26;
                }
            }

            if (type == "Land")
            {
                propSubId = 20;
            }

            if (type == "Maisonette")
            {
                if (style == "Ground Floor")
                {
                    propSubId = 10;
                }

                else
                {
                    propSubId = 11;
                }
            }

            return propSubId;
        }
    }
}
