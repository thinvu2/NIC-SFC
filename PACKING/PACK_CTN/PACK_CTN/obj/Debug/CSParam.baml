   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.CSParam   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��csparam��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FFE5F2F5 � Title$ Codesoft Parameter��5   	    � WindowStartupLocation$ CenterScreen=�6&    � WindowState$ Normal=�6K    � WindowStyle$
 None=�6`   $	��600��6s   $	��820��6�   $	��860��6�   $
��1400��6�   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+ �� ElementName$ csparam��6K   5      � 5   
   $��1��6    5      �� 5      $
��auto �6    60   �� 5       6"   .+�5"      � $��0��6   ���	#FFB1BBBB$��0 2 0 0q�67   	 � ColumnDefinitions	 5#      �� 5$      $
��auto �6'   66   �� 5%      6)   .+�5'      �� $��Label :��6   6-   �� 5(      $��1��6   
�������
���   �$��1q�6P   ._�5)      �� -   5        $��urFile��5)      6.   5*      5+      � 5,      $��1��6   	 5-      �� 5.      $
��191* �6'   �� 5/      $
��569* �6'   .+�51      &   System.Windows.Controls.DataGrid  -   5        
  Name$
 dataGridcsParam��51      $��2��64   ��6H   �� 52      53      54      55      