   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.CheckLabel   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��CheckLabelName��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FF8DAEB6 � Title$ 
CheckLabel��5   	   $	��250��6   $	��400��6)    � WindowState$ Normal=�65    � WindowStyle$
 None=�6J    � WindowStartupLocation$ CenterScreen=�5   <    � 	Resources 5       
 header �� Color  
Label_text  	Colortext  Icon��%�   &	       &	 !     &	 B     &	 e     �� 5   
   $ #0889a6��6)   �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ #0889a6��6'   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5    
   � $��0��6   .+�5!      *   $PACK_CTN.UserControlCTN.ControlBarUC  
	 � Tag	 6   �� *Title+
 �� ElementName$
 CheckLabelName��6R   5"      � 5#   
   $��1��6   ��5$      �� 5%       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5&      �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5'      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5(       5*      �� 5+      $��50 �6    6.   �� 5,      6"   .+�5.      � $��0��6   #��[ 6!   .+�5/      �� $��CHECK LABEL��6   $��30(�6.   $��Center=�6<   $��Center=�6W   
�������6�   50      � 51      $��1��6    � ColumnDefinitions 52      �� 53      $	��372 �6'   66   .+�55      � $��0��6   $��2��6'    56      �� 57      6-   .+�59      � $��0��6   $��5 0 0 0q�6+    5:      �� 5;      6.   �� 5<      6.   �� 5=      6.   .+�5?      � $��0��6   $��Bottom=�6,    5@      �� 5A   "   $	��150 �63   6B   �� 5B   "   66   .+�5D      � $��0��6#   5E      � 5F      $��1��6#   .+�5G   "   �� -   5        $��EmpName��5G   (   $��NGUYEN CUONG��67   
��� E��6h   5H      5I      � 5J      $��1��6   $��Center=�6,    5K      �� 5L   "   $	��150 �63   6B   �� 5M   "   66   .+�5O      � $��0��6#   .+�5P   "   �� $��Input EMP No :��6(   
��� E��6[   5Q      � 5R      $��1��6#   .+�5S   "   �� ���	#FF6F5CC7$��1q�6A    �� CornerRadius$ 5��6U   ���#FFFF._�5T   &   G� -   5        $��tbEMP��5T   >   $��30��62    G� PasswordChar$ #��6_   6s   5U   #   5V      5W      � 5X      $��2��6   $��Center=�6,    5Y      �� 5Z   "   $	��150 �63   6B   �� 5[   "   66   .+�5]      � $��0��6#   .+�5^   "   �� $��	Scan SN :��6(   
��� E��6V   5_      � 5`      $��1��6#   .+�5a   "   �� ���	#FF6F5CC7$��1q�6A   $ 5��6U   ���#FFFF._�5b   &   �� -   5        $
��tbSN��5b   :   $��30��6.   6]   5c   #   5d      5e      5g      5h      5i      5j      5k      5l      