#!/bin/bash
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout NZWalks.key -out NZWalks.crt
# 產生無密碼的 pfx 檔案
# openssl pkcs12 -export -out NZWalks.pfx -inkey NZWalks.key -in NZWalks.crt
# 產生有密碼的 pfx 檔案
openssl pkcs12 -export -out NZWalks.pfx -inkey NZWalks.key -in NZWalks.crt -passout pass:Testing123!