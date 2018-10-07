# EolConverter
EolConverter converts all end of line character ocurrences in a byte array taking into acount the encoding.

Supported end of line characters: CR, LF & CRLF. 
Supported file encodings: UTF8, UTF16 Little Endian, UTF16 Big Endian, UTF32 Little Endian & UTF32 Big Endian.

If there is no byte order mark in the array then it tries to find out the encoding from its content.
