   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.CheckCarton   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��CheckCartonName��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FF8DAEB6 � Title$ CheckCarton��5   	   $	��400��6   $	��500��6*    � WindowState$ Normal=�66    � WindowStyle$
 None=�6K    � 	Resources 5       
 header �� Color  
Label_text  	Colortext  Icon��%�   &	       &	 !     &	 B     &	 e     �� 5   
   $ #0889a6��6)   �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ #0889a6��6'   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5    
   � $��0��6   .+�5!      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+	 �� ElementName$	 CheckCartonName��6S   5"      � 5#   
   $��1��6   ��5$      �� 5%      
 �� EndPoint$
 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5&      �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5'      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5(       � ColumnDefinitions 5+      �� 5,      6%   �� 5-      6%   .+�5/      � $��0��6    50      �� 51      6&   �� 52      6&   �� 53      6&   �� 54      $��80 �6$   62   .+�56      �� $��0��6   $��Center=�6*   $��5q�6E   .+�57      �� $��Input Carton No��6   #��[ 66   6`   �� 58      ���	#FF6F5CC7$��1q�65    �� CornerRadius$ 5��6I   ���#FFFF._�59      �� -   5        $��txtCartonNo��59   .   $��30��6"   6^   5:      5;      �� 5<      $��1��6   $��5q�6+   $��Center=�66   .+�5=      �� $��Input Shipping SN��6   #��[ 68   6c   �� 5>      ���	#FF6F5CC7$��1q�65   $ 5��6I   ���#FFFF._�5?      �� -   5        $��txtShippingSN��5?   "   $��30��67   6a   5@      5A      � 5B      $��2��6   .+�5C      �� -   5        $��tbMess��5C       $��WrapWithOverflow=�6.   $��Center=�6N   $��Center=�5D      $��16(�6:   
����  �5F      5G      �� 5H      -   5        $��lbMess��5H   V   ���	#FF6F5CC7$��1q�61   $ 5��6E   $��3��6e   $
��5 15q�6r   ���#0889a6._�5I      �� $��Center=�69   $��Center=�6V   ��6"   Z� .+�5J      �� -   5        $��lblMess��5J   =   $��30(�6    $
��Wait��6.   
����  �6d   5K      5L      5N      � 5O      $��1��6   $��15q�6#   .+�5P      �� -   5        $��lstSN��5P   -   
�������6=   5Q      5R      5S      5T      