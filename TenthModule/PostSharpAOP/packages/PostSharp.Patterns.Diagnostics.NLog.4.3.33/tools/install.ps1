param($installPath, $toolsPath, $package, $project)

. "$toolsPath\functions.ps1"

$psproj = GetPostSharpProject -project $project -create $true 
SetProperty -psproj $psproj -propertyName "LoggingBackEnd" -propertyValue "nlog"

Save -psproj $psproj


$item = $project.ProjectItems.Item("690CAA32-A6FE-41DB-A645-FED34CD38682.txt")
if ($item)
{
  $item.Delete()
}

# SIG # Begin signature block
# MIIR2AYJKoZIhvcNAQcCoIIRyTCCEcUCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUWD655t1Z88cVTTpqvsCeMfwh
# 5Juggg8NMIIE0zCCA7ugAwIBAgIQGNrRniZ96LtKIVjNzGs7SjANBgkqhkiG9w0B
# AQUFADCByjELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDlZlcmlTaWduLCBJbmMuMR8w
# HQYDVQQLExZWZXJpU2lnbiBUcnVzdCBOZXR3b3JrMTowOAYDVQQLEzEoYykgMjAw
# NiBWZXJpU2lnbiwgSW5jLiAtIEZvciBhdXRob3JpemVkIHVzZSBvbmx5MUUwQwYD
# VQQDEzxWZXJpU2lnbiBDbGFzcyAzIFB1YmxpYyBQcmltYXJ5IENlcnRpZmljYXRp
# b24gQXV0aG9yaXR5IC0gRzUwHhcNMDYxMTA4MDAwMDAwWhcNMzYwNzE2MjM1OTU5
# WjCByjELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDlZlcmlTaWduLCBJbmMuMR8wHQYD
# VQQLExZWZXJpU2lnbiBUcnVzdCBOZXR3b3JrMTowOAYDVQQLEzEoYykgMjAwNiBW
# ZXJpU2lnbiwgSW5jLiAtIEZvciBhdXRob3JpemVkIHVzZSBvbmx5MUUwQwYDVQQD
# EzxWZXJpU2lnbiBDbGFzcyAzIFB1YmxpYyBQcmltYXJ5IENlcnRpZmljYXRpb24g
# QXV0aG9yaXR5IC0gRzUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCv
# JAgIKXo1nmAMqudLO07cfLw8RRy7K+D+KQL5VwijZIUVJ/XxrcgxiV0i6CqqpkKz
# j/i5Vbext0uz/o9+B1fs70PbZmIVYc9gDaTY3vjgw2IIPVQT60nKWVSFJuUrjxuf
# 6/WhkcIzSdhDY2pSS9KP6HBRTdGJaXvHcPaz3BJ023tdS1bTlr8Vd6Gw9KIl8q8c
# kmcY5fQGBO+QueQA5N06tRn/Arr0PO7gi+s3i+z016zy9vA9r911kTMZHRxAy3Qk
# GSGT2RT+rCpSx4/VBEnkjWNHiDxpg8v+R70rfk/Fla4OndTRQ8Bnc+MUCH7lP59z
# uDMKz10/NIeWiu5T6CUVAgMBAAGjgbIwga8wDwYDVR0TAQH/BAUwAwEB/zAOBgNV
# HQ8BAf8EBAMCAQYwbQYIKwYBBQUHAQwEYTBfoV2gWzBZMFcwVRYJaW1hZ2UvZ2lm
# MCEwHzAHBgUrDgMCGgQUj+XTGoasjY5rw8+AatRIGCx7GS4wJRYjaHR0cDovL2xv
# Z28udmVyaXNpZ24uY29tL3ZzbG9nby5naWYwHQYDVR0OBBYEFH/TZafC3ey78DAJ
# 80M5+gKvMzEzMA0GCSqGSIb3DQEBBQUAA4IBAQCTJEowX2LP2BqYLz3q3JktvXf2
# pXkiOOzEp6B4Eq1iDkVwZMXnl2YtmAl+X6/WzChl8gGqCBpH3vn5fJJaCGkgDdk+
# bW48DW7Y5gaRQBi5+MHt39tBquCWIMnNZBU4gcmU7qKEKQsTb47bDN0lAtukixlE
# 0kF6BWlKWE9gyn6CagsCqiUXObXbf+eEZSqVir2G3l6BFoMtEMze/aiCKm0oHw0L
# xOXnGiYZ4fQRbxC1lfznQgUy286dUV4otp6F01vvpX1FQHKOtw5rDgb7MzVIcbid
# J4vEZV8NhnacRHr2lVz2XTIIM6RUthg/aFzyQkqFOFSDX9HoLPKsEdao7WNqMIIE
# 1TCCA72gAwIBAgIQJNYk7HqDcrp1pa27PZFLIjANBgkqhkiG9w0BAQsFADB/MQsw
# CQYDVQQGEwJVUzEdMBsGA1UEChMUU3ltYW50ZWMgQ29ycG9yYXRpb24xHzAdBgNV
# BAsTFlN5bWFudGVjIFRydXN0IE5ldHdvcmsxMDAuBgNVBAMTJ1N5bWFudGVjIENs
# YXNzIDMgU0hBMjU2IENvZGUgU2lnbmluZyBDQTAeFw0xNjAyMDIwMDAwMDBaFw0x
# NzA4MTAyMzU5NTlaMG0xCzAJBgNVBAYTAkNaMQ8wDQYDVQQIEwZQcmFndWUxDzAN
# BgNVBAcTBlByYWd1ZTEdMBsGA1UEChQUU2hhcnBDcmFmdGVycyBzLnIuby4xHTAb
# BgNVBAMUFFNoYXJwQ3JhZnRlcnMgcy5yLm8uMIIBIjANBgkqhkiG9w0BAQEFAAOC
# AQ8AMIIBCgKCAQEArHQulfrs9pg3yw4AmwE127XAHfhbzQcj8JTVulrK3TqL+JDe
# RByYw5K3ChCfS8033VZ+OqiPs2MErMUGvCW7DLual3ynDHuBU9yBBmT81CChsMOC
# yX3jUpqxma1de7bXh153SnBNXLt+ju5huQJMw+F4Mo9PgvA9AxEjMiG2t7d97PNk
# ivnNUbUy2DEWVZf+U7SXhr/lRTGK4N0lnT4jO0V0Y5IFov2Vl7lmXzmXkAaGDUcU
# KdmWnwlxG0D7L3YHkZ/xwirhhka35ZiJxvoOyynvXvVD7xuvZeCyYD7eb1BM3wLA
# 8j/suO4teKmqxIRUFNF/yQDwuheJbEx0+frwbwIDAQABo4IBXTCCAVkwCQYDVR0T
# BAIwADAOBgNVHQ8BAf8EBAMCB4AwKwYDVR0fBCQwIjAgoB6gHIYaaHR0cDovL3N2
# LnN5bWNiLmNvbS9zdi5jcmwwYQYDVR0gBFowWDBWBgZngQwBBAEwTDAjBggrBgEF
# BQcCARYXaHR0cHM6Ly9kLnN5bWNiLmNvbS9jcHMwJQYIKwYBBQUHAgIwGQwXaHR0
# cHM6Ly9kLnN5bWNiLmNvbS9ycGEwEwYDVR0lBAwwCgYIKwYBBQUHAwMwVwYIKwYB
# BQUHAQEESzBJMB8GCCsGAQUFBzABhhNodHRwOi8vc3Yuc3ltY2QuY29tMCYGCCsG
# AQUFBzAChhpodHRwOi8vc3Yuc3ltY2IuY29tL3N2LmNydDAfBgNVHSMEGDAWgBSW
# O1PweTOXr32D7y4rzMq3hh5yZjAdBgNVHQ4EFgQUIjZuDn2PEOWMFvuqJdppyQNk
# TmMwDQYJKoZIhvcNAQELBQADggEBAErkJBOqvRZPfxlzdXVEI1Qqt5UkWwcygl89
# vLLFhKSsyyGmm0fe4Tz77pvtbS0HZdMdQjwC6O1UKpEdR3+dDVcn+0PiN1c821uP
# egAfOBmqCwU3t6y2SCMcMBDXbL5ot83aLzxUfGGFZ48GfbdI31MLUNuOGUa1PL9/
# LZMY0WJwzHVTrKIeDqgywpzgBWf3gMyZAMdYl4yF54MSst8Mo1KLKDGd8Ae8dQen
# rgYYVsDqOzZqJ73/r920NX/1NY7GNDM5cBG8F75dOtMw8PLfuf3TP7QUKu2d/i8e
# mqZHOx0U5WSvZ02cKnT4XvrCe7zDmPEWAYtN7ISWUZdToLJw1F0wggVZMIIEQaAD
# AgECAhA9eNf5dklgsmF99PAeyoYqMA0GCSqGSIb3DQEBCwUAMIHKMQswCQYDVQQG
# EwJVUzEXMBUGA1UEChMOVmVyaVNpZ24sIEluYy4xHzAdBgNVBAsTFlZlcmlTaWdu
# IFRydXN0IE5ldHdvcmsxOjA4BgNVBAsTMShjKSAyMDA2IFZlcmlTaWduLCBJbmMu
# IC0gRm9yIGF1dGhvcml6ZWQgdXNlIG9ubHkxRTBDBgNVBAMTPFZlcmlTaWduIENs
# YXNzIDMgUHVibGljIFByaW1hcnkgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkgLSBH
# NTAeFw0xMzEyMTAwMDAwMDBaFw0yMzEyMDkyMzU5NTlaMH8xCzAJBgNVBAYTAlVT
# MR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jwb3JhdGlvbjEfMB0GA1UECxMWU3ltYW50
# ZWMgVHJ1c3QgTmV0d29yazEwMC4GA1UEAxMnU3ltYW50ZWMgQ2xhc3MgMyBTSEEy
# NTYgQ29kZSBTaWduaW5nIENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKC
# AQEAl4MeABavLLHSCMTXaJNRYB5x9uJHtNtYTSNiarS/WhtR96MNGHdou9g2qy8h
# UNqe8+dfJ04LwpfICXCTqdpcDU6kDZGgtOwUzpFyVC7Oo9tE6VIbP0E8ykrkqsDo
# OatTzCHQzM9/m+bCzFhqghXuPTbPHMWXBySO8Xu+MS09bty1mUKfS2GVXxxw7hd9
# 24vlYYl4x2gbrxF4GpiuxFVHU9mzMtahDkZAxZeSitFTp5lbhTVX0+qTYmEgCscw
# dyQRTWKDtrp7aIIx7mXK3/nVjbI13Iwrb2pyXGCEnPIMlF7AVlIASMzT+KV93i/X
# E+Q4qITVRrgThsIbnepaON2b2wIDAQABo4IBgzCCAX8wLwYIKwYBBQUHAQEEIzAh
# MB8GCCsGAQUFBzABhhNodHRwOi8vczIuc3ltY2IuY29tMBIGA1UdEwEB/wQIMAYB
# Af8CAQAwbAYDVR0gBGUwYzBhBgtghkgBhvhFAQcXAzBSMCYGCCsGAQUFBwIBFhpo
# dHRwOi8vd3d3LnN5bWF1dGguY29tL2NwczAoBggrBgEFBQcCAjAcGhpodHRwOi8v
# d3d3LnN5bWF1dGguY29tL3JwYTAwBgNVHR8EKTAnMCWgI6Ahhh9odHRwOi8vczEu
# c3ltY2IuY29tL3BjYTMtZzUuY3JsMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEF
# BQcDAzAOBgNVHQ8BAf8EBAMCAQYwKQYDVR0RBCIwIKQeMBwxGjAYBgNVBAMTEVN5
# bWFudGVjUEtJLTEtNTY3MB0GA1UdDgQWBBSWO1PweTOXr32D7y4rzMq3hh5yZjAf
# BgNVHSMEGDAWgBR/02Wnwt3su/AwCfNDOfoCrzMxMzANBgkqhkiG9w0BAQsFAAOC
# AQEAE4UaHmmpN/egvaSvfh1hU/6djF4MpnUeeBcj3f3sGgNVOftxlcdlWqeOMNJE
# WmHbcG/aIQXCLnO6SfHRk/5dyc1eA+CJnj90Htf3OIup1s+7NS8zWKiSVtHITTuC
# 5nmEFvwosLFH8x2iPu6H2aZ/pFalP62ELinefLyoqqM9BAHqupOiDlAiKRdMh+Q6
# EV/WpCWJmwVrL7TJAUwnewusGQUioGAVP9rJ+01Mj/tyZ3f9J5THujUOiEn+jf0o
# r0oSvQ2zlwXeRAwV+jYrA9zBUAHxoRFdFOXivSdLVL4rhF4PpsN0BQrvl8OJIrEf
# d/O9zUPU8UypP7WLhK9k8tAUITGCAjUwggIxAgEBMIGTMH8xCzAJBgNVBAYTAlVT
# MR0wGwYDVQQKExRTeW1hbnRlYyBDb3Jwb3JhdGlvbjEfMB0GA1UECxMWU3ltYW50
# ZWMgVHJ1c3QgTmV0d29yazEwMC4GA1UEAxMnU3ltYW50ZWMgQ2xhc3MgMyBTSEEy
# NTYgQ29kZSBTaWduaW5nIENBAhAk1iTseoNyunWlrbs9kUsiMAkGBSsOAwIaBQCg
# eDAYBgorBgEEAYI3AgEMMQowCKACgAChAoAAMBkGCSqGSIb3DQEJAzEMBgorBgEE
# AYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMCMGCSqGSIb3DQEJ
# BDEWBBR+C+IB/ofnrDV/tTfQnwrG8kLs3DANBgkqhkiG9w0BAQEFAASCAQCq09WU
# JvaRBIQN+Qq2EGmT1iF/GYQ3J9aURhisCGCfSVb7eO5xRw6VUdI0uzDuFR4G7bwH
# cu4Jcd0B2qlUVJ0nO7YbhUDiHj++ESefC8LZSa8SHCXCGGqkHQNFdnIC5QRA0LxQ
# yI1SbbD8syF1kGlIqUE1kCI6yhcxSvLoC+/z3Y2MoIA59hNACJpq3OR6uleCrLJf
# rX9SDXef6xJA/IBh74CGPoGdb0M/t15w+d12v5rkMuRzrAsXfyrsDf2OSnJ7SQAF
# uxyVAQsaTlZp9BOVchwnXmvjGeyeYxYvRZbc1vfTY0Xb9OuvKNdztEXyOufhsXxk
# Uueb/Nb7AGbRGeOW
# SIG # End signature block
