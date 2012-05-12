﻿<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Dareya" generation="1" functional="0" release="0" Id="42732c5b-6590-467b-99e6-7d6e6f5a728a" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="DareyaGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="DareyaAPI:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Dareya/DareyaGroup/LB:DareyaAPI:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Dareya/DareyaGroup/LB:DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapCertificate|DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/Dareya/DareyaGroup/MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
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
        <lBChannel name="LB:DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapDareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
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
          <role name="DareyaAPI" generation="1" functional="0" release="0" software="C:\Users\Marc Chambers\Documents\Visual Studio 2010\Projects\Daremeto\Dareya\csx\Release\roles\DareyaAPI" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Dareya/DareyaGroup/SW:DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;DareyaAPI&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;DareyaAPI&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Dareya/DareyaGroup/DareyaAPI/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Dareya/DareyaGroup/DareyaAPIInstances" />
            <sCSPolicyFaultDomainMoniker name="/Dareya/DareyaGroup/DareyaAPIFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="DareyaAPIFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="DareyaAPIInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="c7eb8e7d-f549-4fbb-beef-6c2b68d7f924" ref="Microsoft.RedDog.Contract\ServiceContract\DareyaContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="70d2f1a4-485a-40f8-94b8-cc4c54d215ce" ref="Microsoft.RedDog.Contract\Interface\DareyaAPI:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="7ff3cc42-6f47-4758-96df-e493fedcae34" ref="Microsoft.RedDog.Contract\Interface\DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Dareya/DareyaGroup/DareyaAPI:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>