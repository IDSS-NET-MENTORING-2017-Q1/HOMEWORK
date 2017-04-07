<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:b="http://tempuri.org/Books.xsd"
    xmlns:func="http://tempuri.org/Functions"
    exclude-result-prefixes="msxsl">

  <msxsl:script language="CSharp" implements-prefix="func">
    <![CDATA[  
      public string GetDate() {
        return DateTime.Now.ToString("MM/dd/yyyy");
      }
    ]]>
  </msxsl:script>

  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <html>
      <head>
        <title>
          <xsl:value-of select="func:GetDate()"/>
        </title>
      </head>
      <body>
        <h1>
          <xsl:value-of select="func:GetDate()"/>
        </h1>
        <xsl:apply-templates />
      </body>
    </html>
  </xsl:template>

  <xsl:template match="b:catalog">
    <xsl:for-each select="//b:genre">
      <xsl:variable name="preceding" select="preceding::genre/."></xsl:variable>
      <xsl:call-template name="process-genre" />
    </xsl:for-each>

    <h2>
      Total count: <xsl:value-of select="count(b:book)"/>
    </h2>
  </xsl:template>


  <xsl:template name="process-genre">
    <xsl:variable name="genre" select="text()"></xsl:variable>
    <h2>
      <xsl:value-of select="$genre"/>
    </h2>
    <table>
      <tr>
        <th>Author</th>
        <th>Title</th>
        <th>Publishing Date</th>
        <th>Registration Date</th>
      </tr>
      <xsl:for-each select="//b:book[b:genre/text() = $genre]">
        <tr>
          <td>
            <xsl:value-of select="b:author"/>
          </td>
          <td>
            <xsl:value-of select="b:title"/>
          </td>
          <td>
            <xsl:value-of select="b:publish_date"/>
          </td>
          <td>
            <xsl:value-of select="b:registration_date"/>
          </td>
        </tr>
      </xsl:for-each>
      <tr>
        <th></th>
        <th></th>
        <th></th>
        <th>
          <xsl:value-of select="count(//b:book[b:genre/text() = $genre])"/>
        </th>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="text() | @*" />
</xsl:stylesheet>
