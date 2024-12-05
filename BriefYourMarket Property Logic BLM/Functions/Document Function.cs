using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Models;

namespace BriefYourMarketPropertyLogicBLM.Functions
{
    internal class DocumentFunction
    {
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string FormatPropertyRecord(PropertyDataModel property, char endOfField, char endOfRow, string[] fields, string branchId)
        {
            DocumentConverter _documentConverter = new();

            string formattedProperty = $"{branchId.Replace(" ", "")}_{property.Data[0].Value}{endOfField}";

            int epcIndex = -1;
            int fpIndex = -1;

            if (property.Images.Count > 0)
            {
                epcIndex = property.Images[1].Value.IndexOf("EPC Rating Graph");
                fpIndex = property.Images[1].Value.IndexOf("Floorplan");
            }
            
            for (int i = 1; i < fields.Length - 1; i++)
            {
                string databaseField = _documentConverter.GetDatabaseField(fields[i]);

                if (!string.IsNullOrWhiteSpace(databaseField))
                {
                    if (databaseField == "NotMapped")
                    {
                        formattedProperty += _documentConverter.CheckFieldValue(fields[i]) + endOfField;
                    }

                    else
                    {
                        int index = property.Data.FindIndex(p => p.Field == databaseField);
                        formattedProperty += _documentConverter.CheckFieldValue(fields[i], property.Data[index].Value) + endOfField;
                    }
                }

                else
                {
                    if (fields[i].Contains("MEDIA_IMAGE_") || fields[i].Contains("MEDIA_FLOOR_PLAN_") || fields[i].Contains("MEDIA_DOCUMENT_"))
                    {
                        if (fields[i].Contains("MEDIA_IMAGE_"))
                        {
                            int image = int.Parse(fields[i].Remove(0, fields[i].LastIndexOf("_") + 1));

                            if (property.Images.Count > 0 && image < property.Images[0].Value.Count && image != epcIndex && image != fpIndex)
                            {
                                if (fields[i].Contains("TEXT_"))
                                {
                                    formattedProperty += property.Images[1].Value[image] + endOfField;
                                }

                                else
                                {
                                    formattedProperty += property.Images[2].Value[image] + endOfField;
                                }
                            }

                            else if (image == 60 && epcIndex != -1)
                            {
                                if (fields[i].Contains("TEXT_"))
                                {
                                    formattedProperty += property.Images[1].Value[epcIndex] + endOfField;
                                }

                                else
                                {
                                    formattedProperty += property.Images[2].Value[epcIndex] + endOfField;
                                }
                            }

                            else
                            {
                                formattedProperty += endOfField;
                            }
                        }

                        if (fields[i].Contains("MEDIA_FLOOR_PLAN_"))
                        {
                            int image = int.Parse(fields[i].Remove(0, fields[i].LastIndexOf("_") + 1));

                            if (image == 0 && fpIndex != -1)
                            {
                                if (fields[i].Contains("TEXT_"))
                                {
                                    formattedProperty += property.Images[1].Value[fpIndex] + endOfField;
                                }

                                else
                                {
                                    formattedProperty += property.Images[2].Value[fpIndex] + endOfField;
                                }
                            }

                            else
                            {
                                formattedProperty += endOfField;
                            }
                        }

                        if (fields[i].Contains("MEDIA_DOCUMENT_"))
                        {
                            int image = int.Parse(fields[i].Remove(0, fields[i].LastIndexOf("_") + 1));

                            if (image == 0 && epcIndex != -1)
                            {
                                if (fields[i].Contains("TEXT_"))
                                {
                                    formattedProperty += property.Images[1].Value[epcIndex] + endOfField;
                                }

                                else
                                {
                                    formattedProperty += property.Images[2].Value[epcIndex] + endOfField;
                                }
                            }

                            else
                            {
                                formattedProperty += endOfField;
                            }
                        }
                    }

                    else
                    {
                        formattedProperty += endOfField;
                    }
                }
            }

            formattedProperty += endOfRow;

            return formattedProperty;
        }
    }
}
