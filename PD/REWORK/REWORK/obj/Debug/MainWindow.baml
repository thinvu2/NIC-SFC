   M S B A M L   `   `   `  ����A  =REWORK, Version=1.0.4.0, Culture=neutral, PublicKeyToken=nullclr-namespace:REWORKREWORK  5   	   C&clr-namespace:REWORK.UserControlReworkREWORK.UserControlRework  5   	       REWORK.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:REWORK     5   	   J FXamlAnimatedGif, Version=1.2.3.0, Culture=neutral, PublicKeyToken=null<gif2https://github.com/XamlAnimatedGif/XamlAnimatedGif  5   	   /uc&clr-namespace:REWORK.UserControlRework  5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��
mainWindow��5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	    � Title$ Rework CPEII��5   	   $	��760��6   $
��1020��6,   $	��860��69   $
��1400��6I    � WindowStartupLocation$ CenterScreen=�6Z    � WindowStyle$
 None=�6�    � 	Resources 5         item �� Color 
 header  MenuItem  
Label_text  	Colortext  Icon  Text  IconMenu��%T  &	        &	 !     &	 B     &	 c     &	 �     &	 �     &	 �     &	 �     �� 5   
   $ #0889a6��6'   �� 5   
   $ #0889a6��6)   �� 5   
   $ #0889a6��6+   �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ White��6'   �� 5   
   $ White��6'   �� 5   
   $ White��6+   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5       6   .+�5"   
   � $��0��6   .+�5#      +   %REWORK.UserControlRework.ControlBarUC  
 � Tag 6   �� *Title+	 �� ElementName$	 
mainWindow��5$      5%      � 5&   
   $��2��6   $��0,30,0,0q�6!   ��5'      �� 5(      
 �� EndPoint$
 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5)      �  � Color$ 	#FF8ADAE2��6#    � Offset$ 0.975Q�65   � 5+      $ 	#FFF1DABD��6#   $	 0.1Q�65   5,      .+�52      �  53      �� 54      $
��auto �6$   64   �� 55      $	��660��6$   66   �� 56      $��30 �6$   64   .+�58      � $��0��6   
�������.+�59      (  "MaterialDesignThemes.Wpf.ColorZone     Mode$ Inverted=�6/   $��0q�6?   $��0,0,0,0q�6a   $��1��6{   $��Stretch=�6�   ��6K   Z� .��5:      ]� $
��Left=�6$   .+�5;      '  !MaterialDesignThemes.Wpf.PopupBox  $��Right=�66      PlacementMode$ BottomAndAlignRightEdges=�6M   $��Right=�6v      PopupContent. 5<   "   �� ���	#FFC7ECEA
��� E��.@�5=   &   �� -   5        $��itemNameEmp��5=   2   $��Hi: Nguyen Van Cuong��6E   �� 5>   &   -   5        $��itemDbConnected��5>   2   $��	DB: CPEII��6I   �� 5?   &   -   5        $��itemVerPrgram��5?   2   $��Ver: 1.0.0.9��6G   5@   #   5A      �� 5C      -   5        $��btnMain��5D   '   $	��100��5C   =   $��12(�6I   $��1q�6W   $
��Home��5D   6   
���   ���5C   &   Z� 5E      �� 5F      -   5        $��
btnTRework��5G   '   $	��100��5F   =   $��12(�6I   $��1q�6W   $��T Rework��5G   9   
���   ���5F   &   Z� 5H      �� 5L      -   5        $��	btnRepair��5M   '   $	��100��5L   =   $��12(�6J   $��1q�6X   $��Del-R��5M   8   
���   ���5L   &   Z� 5N      �� 5O      -   5        $��btnUpFW��5P   '   $	��100��5O   =   $��12(�6J   $��1q�6X   $��Up FW��5P   6   
���   ���5O   &   Z� 5Q      �� 5R      -	   5        $��
btnProcess��5S   '   $	��120��5R   =   $��12(�6J   $��1q�6X   $��Process Control��5S   9   
���   ���5R   &   Z� 5T      5V      5W      5Z      � 5[      -
   5        $��	panelMain��5[   $   $��1��6   5]      � 5^      $��3��6    � ColumnDefinitions 5_      �� 5`      6-   �� 5a      6-   �� 5b      $
��1.5* �6+   6:   �� 5c      6-   �� 5d      6-   �� 5e      6-   .+�5g      � $��0��6   .+�5h      ]� .+�5i      �� $��Verson: ��6$   $��12(�68   6H   �� 5j      -   5        $��	lblVerson��5j   $   $��1.0.0.0 ��65   $��12(�6H   6Y   5k      5l      � 5m      $��1��6   .+�5n      ]� .+�5o      �� $��	DB Name: ��6$   $��12(�69   6I   �� 5p      -   5        $��	lblDBName��5p   $   $�� ��65   $��12(�6A   6R   5q      5r      � 5s      $��2��6   .+�5t      ]� .+�5u      �� $��
Emp Name: ��6$   $��12(�6:   6J   �� 5v      -   5        $��
lblEmpName��5v   $   $�� ��66   $��12(�6B   6R   5w      5x      � 5y      $��3��6   .+�5z      ]� .+�5{      �� $
��IP: ��6$   $��12(�64   6D   �� 5|      -   5        $��lblIP��5|   $   $��10.224.44.59��61   $��12(�6I   6Y   5}      5~      � 5      $��4��6   .+�5�      ]� .+�5�      �� $��MAC: ��6$   $��12(�65   6E   �� 5�      -   5        $��lblMAC��5�   $   $��50:E5:49:14:CC:DD��62   $��12(�6O   6_   5�      5�      5�      5�      5�      5�      5�      