<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SEOAnalyzer._Default" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>

    <form id="form1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td>Stop-words
                    <asp:TextBox ID="textStopWords" runat="server" TextMode="MultiLine" Width="100%" Rows="3">a about above after again against all am an and any are aren&#39;t as at be because been before being below between both but by can&#39;t cannot could couldn&#39;t did 
didn&#39;t do does doesn&#39;t doing don&#39;t down during
each few for from further had hadn&#39;t has hasn&#39;t have haven&#39;t having he he&#39;d he&#39;ll he&#39;s her here here&#39;s hers herself him himself his how how&#39;s i i&#39;d i&#39;ll i&#39;m
i&#39;ve if in into is isn&#39;t it it&#39;s its itself let&#39;s me more most mustn&#39;t my myself no nor not of off on
once only or other ought our ours ourselves out over own same shan&#39;t she she&#39;d she&#39;ll she&#39;s should shouldn&#39;t
so some such than that that&#39;s the their theirs them themselves then there there&#39;s these they they&#39;d they&#39;ll they&#39;re
they&#39;ve this those through to too under until up very was wasn&#39;t we we&#39;d we&#39;ll we&#39;re we&#39;ve were weren&#39;t
what what&#39;s when when&#39;s where where&#39;s which while who who&#39;s whom why why&#39;s with won&#39;t would
wouldn&#39;t you you&#39;d you&#39;ll you&#39;re you&#39;ve your yours yourselfyourselves</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="checkBoxCalculateWordsOnPage" runat="server" Text="Calculate number of occurrences on the page" Checked="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="checkBoxCalculateKeywordsOnPage" runat="server" Text="Calculate number of occurrences on the page of each word listed in meta tags" Checked="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="checkBoxCalculateExternalLinks" runat="server" Text="Calculate number of external links in the text" Checked="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <input id="radioAnalyzeUrl" type="radio" name="placement" runat="server" checked="true" onchange="var t=document.getElementById('textUrlOrText');t.rows=1;t.innerText='';" />Web page URL<br />
                    <input id="radioAnalyzeText" type="radio" name="placement" runat="server" onchange="var t=document.getElementById('textUrlOrText');t.rows=20;t.innerText='';" />HTML text<br />
                    <br />
                    <asp:Label ID="labelErrorText" runat="server"></asp:Label>
                    <asp:TextBox ID="textUrlOrText" runat="server" TextMode="MultiLine" Rows="1" Width="100%" BorderColor="">http://www.bbc.com/news/</asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:Button ID="buttonAnalyze" runat="server" Text="Analyze" OnClick="Analyze_Click" />
        <br />
        <br />

        <asp:Label ID="labelExternalLinkNumber" runat="server"></asp:Label>
        <table>
            <tr>
                <td valign="top">
                    <asp:GridView ID="gridViewWords" AllowSorting="true" runat="server" OnSorting="gridViewWords_Sorting" AutoGenerateColumns="False" Caption="Words">
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="Word" SortExpression="Key" />
                            <asp:BoundField DataField="Value" HeaderText="Occurences" SortExpression="Value" />
                        </Columns>
                    </asp:GridView>
                </td>
                <td valign="top">
                    <asp:GridView ID="gridViewKeywords" AllowSorting="true" runat="server" OnSorting="gridViewKeywords_Sorting" AutoGenerateColumns="False" Caption="Keywords">
                        <Columns>
                            <asp:BoundField DataField="Key" HeaderText="Word" SortExpression="Key" />
                            <asp:BoundField DataField="Value" HeaderText="Occurences" SortExpression="Value" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </form>
</body>
<script type="text/javascript">
    var t = document.getElementById('textUrlOrText');
    if (document.getElementById('radioAnalyzeUrl').checked)
        t.rows = 1;
    else
        t.rows = 20;
</script>
</html>
