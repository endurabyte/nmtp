sudo apt-get update
sudo apt-get install mingw-w64 mingw-w64-tools autoconf libtool libudev-dev make gettext git -y

mkdir -p input
mkdir -p output

cd input
outDir=$(pwd)/../output

./../buildLibUsb.sh x86_64-w64-mingw32 $outDir
./../buildLibgpg-error.sh x86_64-w64-mingw32 $outDir '--enable-threads=windows'
./../buildLibgcrypt.sh x86_64-w64-mingw32 $outDir
./../buildIconv.sh x86_64-w64-mingw32 $outDir
./../buildLibmtp.sh x86_64-w64-mingw32 $outDir
cd ..
