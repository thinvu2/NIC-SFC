   M S B A M L   `   `   `  ����C  ?PACK_BOX, Version=1.0.6.1, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_BOXPACK_BOX  5   	       PACK_BOX.ShowMessageForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_BOX     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	   -   5        $��ShowMessageForm1��5   	     � Title$  Messages��5   	   $��273.529��6   $��486.397��6+   ���	#FFB83131 � WindowStyle$
 None=�5	   R      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	    � WindowStartupLocation$ CenterScreen=�6\   .��5      �  � RowDefinitions 5   
   �� 5      $	��2.* �6   6+   �� 5      $	��2.* �6   6+   �� 5      6   .+�5   
   � $��0��6   .+�5      �� -   5         ��Name$ 
txbenglish��5        ! MaterialDesignTextAreaTextBox#��[ 5      $��5,3,5,0q�5      $	��Top=�5      $	��100��5       �� AcceptsReturn .  �� TextWrapping$
 Wrap=�5      "	 �� VerticalScrollBarVisibility$
	 Auto=�5      
�������
���  ��5    #   5"      � 5#   
   $��1��6   .+�5$      �� -   5        $ txbvietnamese��5,      #��[ 5%      $��4,8,6,-1q�5&      $	��Top=�5'      $	��100��5(       . $
 Wrap=�5*      $
	 Auto=�5+      
�������5,      5-      � 5.   
   $��2��6   .+�5/      G� -   5        
 G�Name$
 passwordBox��5/      $��2��6/   $��189,10q�6C   $	��Top=�6S   ���	#FFFBF5F56�   50      52      53      