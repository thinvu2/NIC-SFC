   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.IsBracket   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��IsBracketName��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FF8DAEB6 � Title$ 	IsBracket��5   	   $	��400��6   $	��500��6(    � WindowState$ Normal=�64    � WindowStyle$
 None=�6I    � Left$	 820��5   	   
 � Top$	 240��6    � 	Resources 5       
 header �� Color  
Label_text  	Colortext  Icon��%�   &	       &	 !     &	 B     &	 e     �� 5   
   $ #0889a6��6)   �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ #0889a6��6'   5      .��5      � 	 � RowDefinitions	 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5!   
   � $��0��6   .+�5"      *   $PACK_CTN.UserControlCTN.ControlBarUC  

 � Tag
 6   �� *Title+ �� ElementName$ CheckCartonName��6S   5#      � 5$   
   $��1��6   ��5%      �� 5&       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5'      �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5(      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5)       � ColumnDefinitions 5,      �� 5-      $
��241* �6#   62   �� 5.      $
��251* �6#   62   .+�50      � $��0��6   	 51      �� 52      $	��63* �6$   63   �� 53      $	��83* �6$   63   �� 54      $	��39* �6$   63   �� 55      $	��175 �6$   63   .+�57      �� $��0��6   $��Center=�6*   $��5,5,5,4q�6E   $��54��6V   .+�58      �� -   5        $��lbltitle��58   2   $��Carton No: ��6   #��[ 6C   $
��Bold"�6k   �� 59      -   5        $��lblCartonNo1��59   *   $	��123��6   #��[ 6>   $��Center=�6f   �� 5:      -   5        $��lblCartonNo��5:   '   $�� ��6   #��[ 6:   $
��Bold"�6b   5;      �� 5<      $��1��6   $��	5,11,5,12q�6+   $��Center=�6>   $��60��6Y   .+�5=      �� -   5        $��
lblmessage��5=   :   $��Please Scan BRACKET��6   #��[ 6L   $
��Bold"�6t   6�   �� 5>      ���	#FF6F5CC7$��1q�65    �� CornerRadius$ 5��6I   ���#FFFF._�5?      �� -   5        $��txtKPNo��5?   .   $��30��6"   6S   5@      5A      5C      � 5D      $��1��6   $��
0,10,10,15q�6#   	 5E      �� 5F      $��7* �6$   62   �� 5G      $	��90* �6$   63   .+�5I      �� -   5        $��lblCount��5I   1   $��0��6   $��0��6%   #��[ 6A   $
��Bold"�6i   6~   �� 5J      -   5        $��lstBK��5J   :   $��1��6   
�������6:   5K      5L      5M      5N      