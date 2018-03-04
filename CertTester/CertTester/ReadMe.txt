https://www.youtube.com/watch?v=1xtBkukWiek&t=3s

Create Your Own Self Signed X509 Certificate
01) Install OpenSSL (https://www.openssl.org/source/, https://slproweb.com/products/Win32OpenSSL.html)
02) Put OpenSSL bin folder in system path, right-click This PC > properties > Advance System Settings > enviroment variables > edit path (semicolon paste path)
03) Go to command prompt and input OpenSSL to get into it's CLI
04) Type command: req -x509 -days 365 -newkey rsa:2048 -keyout my-key.pem -out my-cert.pem
05) Enter PEM pass phrase (can be anything), used 123456789
06) Enter US as country
07) Common name can be www.modotechsolutions.com
08) Now to make the cert, type command: pkcs12 -export -in my-cert.pem -inkey my-key.pem -out modotech-test-cert.pfx
09) Input the password (123456789) you used previously
10) You keep this cert which contains private key in it, now we create a public key to hand out to clients
11) Type command: pkcs12 -in modotech-test-cert.pfx -clcerts -nokeys -out modotech-test-cert-public.pem
12) Input the password (123456789) you used previously

https://www.youtube.com/watch?v=weiFeQKxqDc&t=5s
X.509 Digital Signature Signing (In C#)

https://www.youtube.com/watch?v=suyKrSRrLwM&t=17s
X.509 Digital Signature Validating (In C#)