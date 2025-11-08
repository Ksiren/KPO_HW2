using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KPO_app2
{
    public abstract class DataImporterTemplate
    {
        public (List<BankAccount>, List<Category>, List<Operation>) ImportData(string filePath)
        {
            var content = ReadFile(filePath);
            var data = ParseData(content);
            return ProcessData(data);
        }

        private string ReadFile(string filePath) => File.ReadAllText(filePath);
        protected abstract object ParseData(string content);

        protected virtual (List<BankAccount>, List<Category>, List<Operation>) ProcessData(object data)
        {
            return (new List<BankAccount>(), new List<Category>(), new List<Operation>());
        }
    }

    public class JsonImporter : DataImporterTemplate
    {
        protected override object ParseData(string content)
        {
            return System.Text.Json.JsonSerializer.Deserialize<JsonElement>(content);
        }

        protected override (List<BankAccount>, List<Category>, List<Operation>) ProcessData(object data)
        {
            var jsonElement = (JsonElement)data;

            var accounts = JsonSerializer.Deserialize<List<BankAccount>>(
                jsonElement.GetProperty("Accounts").GetRawText()) ?? new List<BankAccount>();

            var categories = JsonSerializer.Deserialize<List<Category>>(
                jsonElement.GetProperty("Categories").GetRawText()) ?? new List<Category>();

            var operations = JsonSerializer.Deserialize<List<Operation>>(
                jsonElement.GetProperty("Operations").GetRawText()) ?? new List<Operation>();

            return (accounts, categories, operations);
        }
    }
}
