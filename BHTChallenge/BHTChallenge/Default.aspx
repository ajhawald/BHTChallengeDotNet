<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BHTChallenge._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<%--<div id="headerImageDiv"><h1>Students and Scores</h1></div>--%>

	

	<div>
		<asp:GridView ID="GridView1" runat="server" OnRowDataBound="GridView1_RowDataBound"  GridLines="Both" ></asp:GridView>
	</div>
	<script type="text/javascript">
			$(function(){
			  $('#MainContent_GridView1').tablesorter(); 
		});
		window.onscroll = function() {myFunction()};

		var header = document.getElementById("myHeader");
		var sticky = header.offsetTop;

		function myFunction() {
		  if (window.pageYOffset > sticky) {
			header.classList.add("sticky");
		  } else {
			header.classList.remove("sticky");
		  }
		}
	</script>
</asp:Content>
