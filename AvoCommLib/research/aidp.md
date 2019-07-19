AIDP protocol
=============

UDP, port 3211, max 8192B, up to 3 retries, read timeout 4-40s default 10s

Header:
-------

uint8_t  - `0x01` - SOH
uint32_t - `"AIDP"` - Signature
uint16_t - Sequence ID - Incrementing with every request
uint8_t  - Command ID - `0`-`127` Request, + 128 for Reply
uint32_t - Length

DATA

uint8_t  - `13` - EoF?

Enums:
------

**Error status**:
Ok             = 0
TooBig         = 1
NoSuchName     = 2
BadValue       = 3
ReadOnly       = 4
Generic        = 5
InvalidRequest = 6
ApplianceError = 7
IPInUse        = 8
Timeout        = 4096
BadReply       = 4097
IOError        = 4098

**Model type**:
HP_3x1x16         = 8
Dell_2161DS       = 12
BlackBox_KV121A_E = 15


Commands:
---------

### 1 - Discover Request

if extended {
uint8_t  - `1` - Protocol?
uint16_t - `2`
uint8_t  - `1`
uint8_t  - `8`

if network prefix {
uint8_t  - `2` - Network Prefix
uint16_t - `16`
uint8_t[N] - Network prefix

uint8_t  - `3` - Network Prefix Length
uint16_t - `1`
uint8_t  - Network prefix length
}
}

uint8_t  - `255`

### 2 - Test IP


### 129 - Discover Response

while(!eof) {
uint8_t  - Field ID
uint16_t - Length

ID 1:
  uint16_t - Model ID

ID 2:
  uint8_t[6] - MAC address

ID 3:
  uint8_t[4 || 16] - IP Address

ID 4:
  uint8_t[4] - Subnet address

ID 5:
  uint8_t[4] - Gatewy address

ID 6:
  uint8_t[N] - Hostname

ID 7:
  uint8_t - Supported modes
    0x01 - Static IPv4
    0x02 - Dynamic IPv4
    0x04 - Static IPv6
    0x08 - Dynamic IPv6

ID 255: EOF
}

### 16 - SNMP Get
### 17 - SNMP Get Next

foreach (varbind) {
uint8_t  - `1`
uint16_t - varbind.length
uint8_t[] - varbind.data
}

### 144 - SNMP Get Response
### 145 - SNMP Get Next Response

while(!eof) {
uint8_t - Field ID
uint16_t - Length

ID 1:
  uint16_t - `0`-`5` - Error status

ID 2:
  uint16_t - Error index

ID 3:
  uint8_t[Length] - Data

ID 255: EOF
}
