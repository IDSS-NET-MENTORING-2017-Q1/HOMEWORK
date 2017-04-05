<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:b="http://tempuri.org/Books.xsd"
    exclude-result-prefixes="msxsl">

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <rss version="2.0">
      <xsl:apply-templates />
    </rss>
  </xsl:template>

  <xsl:template match="b:catalog">
    <channel>
      <xsl:apply-templates />
    </channel>
  </xsl:template>

  <xsl:template match="b:book">
    <item>
      <xsl:apply-templates />
    </item>
  </xsl:template>

  <xsl:template match="text() | @*" />
</xsl:stylesheet>
