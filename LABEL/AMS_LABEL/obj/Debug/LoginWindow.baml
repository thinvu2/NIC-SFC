   M S B A M L   `   `   `  ����D  @AMS_Label, Version=1.0.1.1, Culture=neutral, PublicKeyToken=null#clr-namespace:AMSLabelAMSLabel  5   	       AMSLabel.LoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:AMSLabel     5   	   b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � 
ResizeMode$  NoResize=�5   	    � WindowStyle$
 None=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowState$ 	Maximized=�5   	   .��5      �� $��3q�6    �� CornerRadius$ 35��6!   ._�5   
   � .+�5      �  � ColumnDefinitions 5      �� 5      $	��63* �6'   �� 5      $
��731* �6'   .+�5      � $��2��6   .+�5      �� $��/Assets/12345.jpg��6   $
��Fill=�68   5      � 5      
������  J� Opacity$	 0.7Q�60   $��2��6>   5      � 5      
������ $��2��60   $��0,-10,-0.4,10.4q�6D   .+�5      #  MaterialDesignThemes.Wpf.Card  +  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth5=�6*   $��250 70q�6[   ���#000$	 0.6Q�6}   5        5      $ Depth5=�6*   $��250 70q�6[   
������ .��5      �  � RowDefinitions 5       �� 5!   "   $
��auto �60   6@   �� 5"   "   $
��auto �60   6@   �� 5#   "   $
��auto �60   6@   �� 5$   "   $
��auto �60   6@   �� 5%   "   $
��auto �60   6@   �� 5&   "   62   �� 5'   "   $
��auto �60   6@   �� 5(   "   $
��auto �60   6@   .+�5*      � $��0��6#   .+�5+   "   �� $��	0 30 0 20q�6,   $��24(�6?   $��Arial)�6M   $��Center=�6`   ���#fffLogin to your account5-   #   5.      � 5/      $��1��6#   $��20 10q�60   ���#FFF 50   "   �� 51   &   $��50 �67   6D   �� 52   &   $��* �67   6C   .+�54   "   � $��0��6'   .+�55   &   '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 Account=�6>   $��25��6M   $��25��6Y   $��Center=�6d   $��Center=�6   56   #   � 57   "   $��1��6'   .+�58   &   �� -   5        
 ��Name$
 	txt_empNo��5;   )   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Emp no��59   !   $��0q�5:   !   .  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton .  !  MaterialDesignOutlinedTextBox#��[  5=      6>   5>   #   5?      � 5@      $��2��6#   $��20 10q�60   ���#FFF 5A   "   �� 5B   &   $��50 �67   6D   �� 5C   &   $��* �67   6C   .+�5E   "   � $��0��6'   .+�5F   &     $
	 Lock=�6>   $��25��6J   $��25��6V   $��Center=�6a   $��Center=�6|   5G   #   � 5H   "   $��1��6'   .+�5I   &   �� .+�5J   *   G� -   5         G�Name$ txt_Password��5K   %   $��0q�5L   %    % !MaterialDesignOutlinedPasswordBox#��[ 5M       . $	��Top=�5O      $ Password��5P         HelperTextFontSize$ 30Q�5Q   %   5R   '   5S   #   5T      � 5U      $��3��6#   $
��20 5q�60   � 5V      $��4��6#   $
��20 5q�60    5W   "   �� 5X   &   $	��80* �67   �� 5Y   &   $	��43* �67   .+�5[   "   �� -   5         �� 	IsDefault . $��	0,2,0,-11q�5[   O   $��2��6b   $��40��6v   ��5\   &   �� 5]   *    �� 
StartPoint$	 0,0&�6?    �� EndPoint$	 1,1&�6P   .��5^   .   �  � Color$ #2e7d32��6;    � Offset$ 1Q�6K   � 5_   .   $ White��6;   $ 0Q�6I   � 5`   .   $ #2e7d32��6;   $	 0.9Q�6K   5a   +   ��5c   &   �� 5d   *    �� Triggers 5e   .   P� 5f   2    P� Property �@J�IsMouseOver P� Value . . �5g   6   ��  �� Property � �� �� Value
 ��ح�6Z   5h   3   5j   +   .��5l   &   ��  �� Orientation$ 
Horizontal=�61   .+�5m   *     $��Center=�6B   $	 Login=�6]   
���   �$��Arial)�6}   $��18(�6�   $
��Bold"�6�   �� 5n   *   $��Center=�64   
���   �$��Arial)�6b   $��18(�6u   $
��Bold"�6�   LOGIN5p   +   5q   '   5s   #   5u      � 5�      $��6��6$   � 5�      $��7��6#   $
��20 5q�60   .+�5�   "   �� $��Center=�6-   $ 
Horizontal=�6H   5�      5�      5�      5�      5�      � 5�      ��5�      �� 5�      $	 0,0&�6*   $	 1,1&�6;   .��5�      � $ #f7d2d7��6'   $ 0Q�67   � 5�      $ White��6'   $	 0.5Q�65   � 5�      $ #00861f��6'   $	 1.5Q�67   5�      .+�5�      � -   5         �Name$ ProgressbarCicle��5�      $	��150��69   $	��150��6F   ���#008168 % !MaterialDesignCircularProgressBar#��[ 5�       � Value$ 0Q�5�       � IsIndeterminate . 5�      5�      5�      5�      5�      