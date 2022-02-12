sudo apt-get update

sudo apt-get install pkg-config autoconf libtool libudev-dev make gettext git -y

mkdir input
cd input

git clone https://github.com/libusb/libusb.git
cd libusb/
git checkout v1.0.25

./autogen.sh --host=x86_64-linux-gnu --prefix=/usr/x86_64-linux-gnu
make
make install
DESTDIR=$HOME/output/ make install
cd ..

git clone https://github.com/gpg/libgpg-error.git
cd libgpg-error/
git checkout libgpg-error-1.42

./autogen.sh
./configure --host=x86_64-linux-gnu --prefix=/usr/x86_64-linux-gnu
make
make install
DESTDIR=$HOME/output/ make install
cd ..

git clone https://github.com/gpg/libgcrypt.git
cd libgcrypt/
git checkout libgcrypt-1.9.3

./autogen.sh
./configure --host=x86_64-linux-gnu --with-gpg-error-prefix=/usr/x86_64-linux-gnu --prefix=/usr/x86_64-linux-gnu --disable-asm
make
make install
DESTDIR=$HOME/output/ make install
cd ..

wget -c https://ftp.gnu.org/pub/gnu/libiconv/libiconv-1.16.tar.gz -O - | tar -xz
cd libiconv-1.16
./configure --host=x86_64-linux-gnu --prefix=/usr/x86_64-linux-gnu
make
make install
DESTDIR=$HOME/output/ make install
cd ..

git clone https://github.com/shaosss/libmtp.git
cd libmtp/
git checkout  master-custom

cp /usr/x86_64-w64-mingw32/bin/libiconv-2.dll ./libiconv-2.dll
printf 'n\n' | PKG_CONFIG_LIBDIR=/usr/x86_64-linux-gnu/lib/pkgconfig/ ./autogen.sh --host=x86_64-linux-gnu --prefix=/usr/x86_64-linux-gnu
make
DESTDIR=$HOME/output/ make install
cd ..

cd ..
cp -R ~/output ./
