   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.CloseCarton   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��CloseCartonName��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FFE5F2F5 � Title$ Close Carton��5   	   $	��300��6   $	��400��6,    � WindowState$ Normal=�68    � WindowStartupLocation$ CenterScreen=�6N    � WindowStyle$
 None=�6s   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+ �� ElementName$ CloseCartonName��6V   5      � 5   
   $��1��6   ��5      �� 5      	 �� EndPoint$	 0.5,1&�6&   
 �� 
StartPoint$
 0.5,0&�67   .��5       �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5!      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5"       5$      �� 5%      6"   �� 5&      6"   �� 5'      6"   .+�5)      � $��0��6   $��Bottom=�6     � ColumnDefinitions 5*      �� 5+      6)   �� 5,      6)   .+�5.      �� $��0��6   $��SN / Carton No :��6(   
��� E��$��Right=�6[   $��0 0 15 0q�6w   $��Center=�6�   6�   �� 50       �� CornerRadius$ 5��6   ���	#FF6F5CC7$��1q�6B   $
��Left=�6W   $��1��6r   ���#ffff$	��160��6�   $��35��6�   ._�51      �� -   5        $��	InputData��51      $��16(�60   6\   52      53      � 54      $��1��6    55      �� 56      6)   �� 57      6)   .+�5:      �� $��0��6   $��Label QTY :��6(   
��� E��$��Right=�6V   $��0 0 15 0q�6r   $��Center=�6�   6�   �� 5<      $��1��6   $ 5��6)   
����������	#FF6F5CC7$��1q�6f   $��80��6z   $
��Left=�6�   $��35��6�   ._�5=      �� .+�5>      �� -   5        $��cbbLbQTY��5>   #    �� 
IsEditable .  �� MaxDropDownHeight$	 150��6j   $��24,0,5,0q�6�   ��63   �� *MyDataSource+6�   5?      5@      5A      � 5B      $��2��6    5C      �� 5D      6)   �� 5E      6)   .+�5H      �� -   5        $��0��5H       # MaterialDesignRaisedLightButton#��[ 6)   $��90��6c   $��Right=�6o   
�������
��� E��$��OK��5I   0   $��5q�5J       � ToolTip$4 .Resource name: MaterialDesignRaisedLightButton��6$   5L      �� 5M      -   5        $��1��5M      #��[ 6)   $��90��6c   $
��Left=�6o   
�������
��� E��$��5q�5N      $4 .Resource name: MaterialDesignRaisedLightButton��6$   $��Cancle��6]   5O      5P      5Q      5R      5S      