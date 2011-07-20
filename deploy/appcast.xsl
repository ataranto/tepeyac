<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/data">
        <rss version="2.0"
             xmlns:sparkle="http://www.andymatuschak.org/xml-namespaces/sparkle"
             xmlns:dc="http://purl.org/dc/elements/1.1/">
           <channel>
              <title>Tepeyac</title>
              <link>http://ataranto.github.com/tepeyac/appcast.xml</link>
              <description>Tepeyac</description>
              <language>en</language>      
                 <item>
                    <title>Tepeyac <xsl:value-of select="version" /></title>
                                <sparkle:releaseNotesLink>
                                    http://opensword.org/Pixen/Help/pgs/rnotes.html
                                </sparkle:releaseNotesLink>
                    <pubDate><xsl:value-of select="date" /></pubDate>
                    <enclosure url="https://github.com/downloads/ataranto/tepeyac/Tepeyac.tar.bz2"
                               type="application/octet-stream">
                      <xsl:attribute name="sparkle:version">
                        <xsl:value-of select="version" />
                      </xsl:attribute>
                      <xsl:attribute name="length">
                        <xsl:value-of select="length" />
                      </xsl:attribute>
                      <xsl:attribute name="sparkle:dsaSignature">
                        <xsl:value-of select="signature" />
                      </xsl:attribute>
                    </enclosure>
                 </item>
           </channel>
        </rss>
    </xsl:template>
</xsl:stylesheet>
