   M S B A M L   `   `   `  ����G  CCodeSoft_New, Version=1.0.1.4, Culture=neutral, PublicKeyToken=null1clr-namespace:CodeSoft_9CodesCodeSoft_9Codes  5   	   %    CodeSoft_9Codes.ShowMessageForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:CodeSoft_9Codes     5   	   -   5          �Name$  ShowMessageForm1��5   	   
��� ���$��Regular"�5
   	   $��14(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	   $��Times New Roman)�5   	    � WindowStartupLocation$ CenterScreen=�6G    � Title$ ShowMessageForm��5   	   $��273.529��6!   $��486.397��62   .��5      � ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�63   .��5      �  � Color$ 	#FF490909��6    � Offset$ 0.004Q�61   � 5      $ 	#FFEC0E0E��6   $ 1Q�61   5      	 � RowDefinitions	 5   
   �� 5      $	��2.* �6   6+   �� 5      $	��2.* �6   6+   �� 5      6   .+�5   
   � $��0��6   .+�5      �� -   5        
 ��Name$
 
txbenglish��5"      $��5,5,5,5q�5       �� AcceptsReturn .  �� TextWrapping$
 Wrap=�5       " �� VerticalScrollBarVisibility$
 Auto=�5!      
�������
���  ��5"   #   5$      � 5%   
   $��1��6   .+�5&      �� -   5        $
 txbvietnamese��5+      $��5,5,5,5q�5'       . $
 Wrap=�5)      $
 Auto=�5*      
�������5+      5,      � 5-   
   $��2��6   .+�50      �� -   5         ��Name$ btnClose��50      $��2��6'   $��Times New Roman)�6;   $��25(�6X   ���#034E70
�������$
��Bold"�6�   $��190,7q�6�   $��50��6�   $��OK��6�   $��Center=�6�   ��51      �� 52       �� 
TargetType" ��6   .�53      ��  �� Property � �� �� Value �#034E7067   �� 54       � �� 55      �� 56   "    �� 
TargetType" ��62   .R�57   &   ��  ��Name$ Border��6-   #��z��6=   ._�58   *   �� $��Center=�6;   $��Center=�6X   59   '    �� Triggers 5:   &   P� 5;   *    P� Property �@J�IsMouseOver P� Value . . �5<   .   ��  �� 
TargetName$ Border��6[    � �� �#0889A65=   +   5?   #   5A      5B      5E      5F      5G      5H      