git clone https://github.com/endurabyte/libmtp.git
cd libmtp/
git checkout master-libmtpsharp

printf 'n\n' | PKG_CONFIG_LIBDIR=$2/lib/pkgconfig/ LDFLAGS=-L$2/lib CPPFLAGS=-I$2/include ./autogen.sh --host=$1 --prefix=$2
make
make install
cd ..
