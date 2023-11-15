brew update

brew install pkg-config
brew install autoconf
brew install libtool
brew install libudev-dev
brew install make
brew install gettext
brew install git
brew install automake
brew install wget

mkdir input
mkdir output

cd input
outDir=$(pwd)/../output

../../buildLibUsb.sh aarch64-apple-darwin $outDir
../../buildLibgpg-error.sh aarch64-apple-darwin $outDir
../../buildLibgcrypt.sh aarch64-apple-darwin $outDir
../../buildIconv.sh aarch64-apple-darwin $outDir
../../buildLibmtp.sh aarch64-apple-darwin $outDir
cd ..
