# Local development
az login -t d9f3dee8-148c-49ea-8e87-dd97cd0cd5de
az account set -s 7a9a0f8c-eb0d-4803-89f5-4e9e32a6333d
az acr login -n dxpprivate

$trackingEndpoint = 'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/fnc/mst/messaging/rabbitmq?code=xKvUgzJgdbwRcBRvPsee3gPbmBMXTpR8pkWhTWQky6RfvW5cxe5kqn94C9D'
Invoke-WebRequest $trackingEndpoint | Set-Content './rabbitmq/rabbitmq-definitions.json'

$env:CAKE_SETTINGS_SKIPPACKAGEVERSIONCHECK="true"
.\build.ps1 --target=Compose
.\build.ps1 --target=Up

newman run -k -e ./tests/IntegrationTest/AppData/Docker.postman_environment.json ./tests/IntegrationTest/AppData/Test.postman_collection.json

$postParams = Get-Content './rabbitmq/rabbitmq-definitions.json'
Invoke-WebRequest -Uri $trackingEndpoint -Method POST -Body $postParams

.\build.ps1 --target=Down

# JAEGER: http://localhost:16686/search
# Grafana: http://localhost:3005
docker exec -it elasticsearch /usr/share/elasticsearch/bin/elasticsearch-reset-password -u elastic
docker exec -it elasticsearch /usr/share/elasticsearch/bin/elasticsearch-create-enrollment-token -s kibana
docker cp elasticsearch:/usr/share/elasticsearch/config/certs/http_ca.crt .

# Generate secret
using System;
using System.Text;
using System.Security.Cryptography;
					
public class Program
{
	public static void Main()
	{
		string input = "xsdLservCCfurN0uz65k8GL9Dd6tC2Wzn4EDRpmK";
		using (SHA256 shA256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            Console.WriteLine(Convert.ToBase64String(((HashAlgorithm)shA256).ComputeHash(bytes)));
        }
	}
}


"$timestamp" => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
"$randomDateRecent" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
"$guid" => Guid.NewGuid().ToString(),
"$randomFirstName" => Guid.NewGuid().ToString("N"),
"$randomEmail" => $"{Guid.NewGuid().ToString("N")}@email.com",
"$randomUrl" => $"https://{Guid.NewGuid().ToString("N")}.com",
"$randomInt" => new Random().Next(100, 999).ToString()