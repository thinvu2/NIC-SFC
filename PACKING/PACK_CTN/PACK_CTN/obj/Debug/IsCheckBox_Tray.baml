   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.IsCheckBox_Tray   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   @UserControlCTN%clr-namespace:PACK_CTN.UserControlCTN     5   	   -   5          � Title$  IsCheckBox_Tray��5	   	   $	��450��6!   $��732.5��6.    � WindowStartupLocation$ CenterScreen=�6<    � 	Resources 5
         	Colortext �� Color��%-   &	        �� 5   
   $ 	OrangeRed��6,   5      .��5      � $��0,0,2,0q�6   .+�5   
   � $��0��6   $��0,0,300,385q�6    � ColumnDefinitions 5      �� 5      $
��139* �6#   �� 5      $
��107* �6#   5      � 5   
   ��5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5       5      �� 5      $	��35* �6#   �� 5      $
��320* �6#   �� 5       $
��369* �6#   .+�5"      � $��0��6   $��2��6#   	 � RowDefinitions	 5#      �� 5$      $	��63* �6$   �� 5%      $	��83* �6$   �� 5&      $	��39* �6$   �� 5'      $	��175 �6$   .+�5)      �� $��0��6   $��Center=�6*   $��5,5,5,4q�6E   $��54��6V   .+�5*      �� $��	Tray No: ��6   #��[  60   $
��Bold"�6X   $��0,0,60,0q�6j   �� 5+      -   5        
 ��Name$
 lblCartonNoSN_SSN1��5+   *   $	��123��6   #��[  6F   $��Center=�6n    J� RenderTransformOrigin$ -0.897,0.615&�6�   �� 5,      -   5        $
 lblCartonNoSN_SSN��5,   '   $�� ��6   #��[  6B   $
��Bold"�6j   5-      �� 5.      $��1��6   $��	5,11,5,12q�6+   $��Center=�6>   $��60��6Y   .+�5/      �� -   5        $
 lblmessageSN_SSN��5/   9   $��Please Scan SN/SSN��6   #��[  6S   $
��Bold"�6{   �� 50      ���	#FF6F5CC7$��1q�65    �� CornerRadius$ 5��6I   ���#FFFF$��0,0,64,0q�6m   ._�51      �� -   5         ��Name$ 	txtSN_SSN��51   .   $��30��6"   $��0,0,1,0q�6Y   52      53      55      � 56      $��2��6   $��
0,10,10,15q�6#   	 57      �� 58      $��7* �6$   �� 59      $	��90* �6$   .+�5;      �� -   5        $
 lblCountSN_SSN��5;   1   $��0��6   $��0��6%   #��[  6I   $
��Bold"�6q   �� 5<      -   5         ��Name$ lstView��5<   :   $��1��6   
�������$��0,0,10,0q�6K   5=      5>      5@      5A      