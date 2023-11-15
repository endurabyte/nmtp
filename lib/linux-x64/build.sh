sudo apt-get update
sudo apt-get install pkg-config autoconf libtool libudev-dev make gettext git -y

mkdir -p input
mkdir -p output

cd input
outDir=$(pwd)/../output

../../buildLibUsb.sh x86_64-linux-gnu $outDir
../../buildLibgpg-error.sh x86_64-linux-gnu $outDir
../../buildLibgcrypt.sh x86_64-linux-gnu $outDir
../../buildIconv.sh x86_64-linux-gnu $outDir
../../buildLibmtp.sh x86_64-linux-gnu $outDir
cd ..
