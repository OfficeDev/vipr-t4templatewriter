#!/bin/bash

SDKS_PATH="../../Office-365-SDK-for-iOS"


rm -rf "$SDKS_PATH"/sdk

cp -R ios_sdks_out/ "$SDKS_PATH"
cd "$SDKS_PATH/test/sdk_tests/"

rm -rf Podfile.lock
rm -rf Pods

pod install
 
