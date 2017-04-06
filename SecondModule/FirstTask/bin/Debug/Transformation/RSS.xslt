<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:b="http://tempuri.org/Books.xsd"
    exclude-result-prefixes="msxsl">

  <xsl:output method="xml" indent="yes"/>

  <xsl:template name="process-isbn-link">
    <xsl:param name="isbn"></xsl:param>
    <xsl:param name="genre"></xsl:param>

    <xsl:if test="$isbn and $genre = 'Computer'">
      <xsl:variable name="url">
        <xsl:value-of select="concat('https://www.safaribooksonline.com/search/?query=', $isbn)"/>
      </xsl:variable>
      <link>
        <xsl:value-of select="$url"/>
      </link>
    </xsl:if>
  </xsl:template>

  <xsl:template match="/">
    <rss version="2.0">
      <xsl:apply-templates />
    </rss>
  </xsl:template>

  <xsl:template match="b:catalog">
    <channel>
      <title>Books RSS</title>
      <description>Books collection</description>
      <link>http://tempuri.org/Books</link>
      <xsl:apply-templates />
    </channel>
  </xsl:template>

  <xsl:template match="b:book">
    <item>
      <xsl:apply-templates />
      <xsl:call-template name="process-isbn-link">
        <xsl:with-param name="genre" select="b:genre/text()"></xsl:with-param>
        <xsl:with-param name="isbn" select="b:isbn/text()"></xsl:with-param>
      </xsl:call-template>
    </item>
  </xsl:template>

  <xsl:template match="b:genre">
    <category>
      <xsl:value-of select="." />
    </category>
  </xsl:template>

  <xsl:template match="b:title">
    <title>
      <xsl:value-of select="." />
    </title>
  </xsl:template>

  <xsl:template match="b:author">
    <author>
      <xsl:value-of select="." />
    </author>
  </xsl:template>

  <xsl:template match="b:description">
    <description>
      <xsl:value-of select="."/>
    </description>
  </xsl:template>

  <xsl:template match="b:id">
    <guid>
      <xsl:value-of select="." />
    </guid>
  </xsl:template>

  <xsl:template match="b:registration_date">
    <pubDate>
      <xsl:value-of select="." />
    </pubDate>
  </xsl:template>

  <xsl:template match="text() | @*" />
</xsl:stylesheet>
