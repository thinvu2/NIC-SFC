   M S B A M L   `   `   `  ����F  BMake_Weight, Version=1.0.5.2, Culture=neutral, PublicKeyToken=null)clr-namespace:Make_WeightMake_Weight  5   	       Make_Weight.SetupStation   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Make_Weight     5   	   LmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes     5   	   Hi:http://schemas.microsoft.com/expression/2010/interactivity     5	   	   -   5        $��staionsetup��5   	      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FFE5F2F5 � Title$ SetupStation��5   	   $	��300��6   $	��400��6+    � WindowState$ Normal=�67    � WindowStyle$
 None=�6L    � WindowStartupLocation$ CenterScreen=�5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��1��6   ���#0889a6 5      �� 5      6"   �� 5      $
��0.5* �6    60   �� 5      6"   .+�5      � $��0��6   
�������$��	2 1 2 0.5q�63    � ColumnDefinitions 5      �� 5       $��2* �6'   64   �� 5!      $��3* �6'   64   .+�5#      � $��0��6   $
��10 0q�6'   .+�5$      �� -   5        $��cbChangeLine��5$      ���	#DDD12424���	#FF29D3EE���	#FF52CAD66{   �� 5%      $��Center=�6   
��� E��$��Center=�6P   $��Change Line��6k   6�   5&      � 5'      $��1��6   $
��10 0q�6'   .+�5(      �� -   5        $��cbbLineName��5(      $��Center=�62   
��� E�� �� 
IsEditable . 	 �� MaxDropDownHeight$		 150��6�   ��6d   �� *MyDataSource+6�   5)      5*      � 5+      $��1��6   
�������$��2 0.5q�63    5,      �� 5-      $	��71* �6'   �� 5.      $
��124* �6'   .+�50      � $��2��6   .+�51      �� $��Center=�6   $��Center=�68   $��30��6U   $��Input pass :��6a   
��� E��6�   G� 53      -   5        $��IputPassword��53   #   $��Center=�67   $��Right=�6R   
 G� PasswordChar$
 #��6n   $	��100��6   $��0,0,26,7q�6�   
����������#0889a6
���   �6�   55      56      � 58      $��3��6   
�������$��	2 0.5 2 1q�63    59      �� 5:      6)   �� 5;      6)   .+�5=      � $��0��6   .+�5>      �� -   5        $	��100��5>      $��OK��6)   ���#0889a6
�������6v   5?      � 5@      $��1��6   .+�5A      �� -   5        $	��100��5A      $��Cancle��6)   ���#0889a6
�������6}   5B      5C      5E      5G      5H      