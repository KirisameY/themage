extends GPUParticles3D

func _on_bullet_tree_exited() -> void:
	emitting = false
	await get_tree().create_timer(1).timeout
	queue_free()
