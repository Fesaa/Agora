import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-agora-button',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './agora-button.component.html',
  styleUrls: ['./agora-button.component.css']
})
export class AgoraButtonComponent {
  @Input() type: 'primary' | 'secondary' | 'text' = 'primary';
  @Input() icon?: string;
  @Input() disabled = false;
  @Input() routerLink?: string;

  @Output() onClick = new EventEmitter<void>();

  handleClick(event: Event): void {
    if (!this.disabled) {
      this.onClick.emit();
    }
  }
}
