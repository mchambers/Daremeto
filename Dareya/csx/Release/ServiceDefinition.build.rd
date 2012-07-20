<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Dareya" generation="1" functional="0" release="0" Id="2ef14283-4bcb-4a30-b50d-8528c38d4f29" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="DareyaGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="DareyaAPI:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Dareya/DareyaGroup/LB:DareyaAPI:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="DareyaAPI:https" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/Dareya/DareyaGroup/LB:DareyaAPI:https" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|DareyaAPI:api.dareme.to" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapCertificate|DareyaAPI:api.dareme.to" />
          </maps>
        </aCS>
        <aCS name="DaremetoWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDaremetoWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DaremetoWorker:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDaremetoWorker:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="DaremetoWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDaremetoWorkerInstances" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="DareyaAPIInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPIInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:DareyaAPI:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:DareyaAPI:https">
          <toPorts>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI/https" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCertificate|DareyaAPI:api.dareme.to" kind="Identity">
          <certificate>
            <certificateMoniker name="/Dareya/DareyaGroup/DareyaAPI/api.dareme.to" />
          </certificate>
        </map>
        <map name="MapDaremetoWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DaremetoWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDaremetoWorker:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DaremetoWorker/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapDaremetoWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Dareya/DareyaGroup/DaremetoWorkerInstances" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDareyaAPI:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapDareyaAPIInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Dareya/DareyaGroup/DareyaAPIInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="DaremetoWorker" generation="1" functional="0" release="0" software="C:\Users\Marc Chambers\Documents\Visual Studio 2010\Projects\Daremeto\Dareya\csx\Release\roles\DaremetoWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;DaremetoWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;DaremetoWorker&quot; /&gt;&lt;r name=&quot;DareyaAPI&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;https&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Dareya/DareyaGroup/DaremetoWorkerInstances" />
            <sCSPolicyFaultDomainMoniker name="/Dareya/DareyaGroup/DaremetoWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="DareyaAPI" generation="1" functional="0" release="0" software="C:\Users\Marc Chambers\Documents\Visual Studio 2010\Projects\Daremeto\Dareya\csx\Release\roles\DareyaAPI" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="3584" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="https" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/Dareya/DareyaGroup/DareyaAPI/api.dareme.to" />
                </certificate>
              </inPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;DareyaAPI&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;DaremetoWorker&quot; /&gt;&lt;r name=&quot;DareyaAPI&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;https&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0api.dareme.to" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Dareya/DareyaGroup/DareyaAPI/api.dareme.to" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="api.dareme.to" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Dareya/DareyaGroup/DareyaAPIInstances" />
            <sCSPolicyFaultDomainMoniker name="/Dareya/DareyaGroup/DareyaAPIFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="DaremetoWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="DareyaAPIFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="DaremetoWorkerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="DareyaAPIInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="29de44b0-e2ee-44ae-86c8-8ab67731e13c" ref="Microsoft.RedDog.Contract\ServiceContract\DareyaContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="850e9ac7-665b-4350-8afb-e2b8821a1034" ref="Microsoft.RedDog.Contract\Interface\DareyaAPI:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="92ef39e0-3c52-42a1-bf45-3128caeffcda" ref="Microsoft.RedDog.Contract\Interface\DareyaAPI:https@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI:https" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>